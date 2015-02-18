    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Assembler {
    class Program {
        static void Main( string[] args ) {
            StreamReader input;
            StreamWriter output;
            parseArguments( args, out input, out output );

            Parser parser = new Parser( input );
            SymbolTable symbols = new SymbolTable();

            findLabels( parser, symbols );

            assemble( parser, symbols, output );

            output.Close();
        }

        private static void parseArguments( string[] args, out StreamReader input, out StreamWriter output ) {
            string in_file = String.Empty;
            string out_file = String.Empty;
            bool test_flag = false;
            Match matches;

            Regex argument_patterns = new Regex( @"^([-]t)|(.*[.]asm)|(^[-]o=(.*))$" );

            for( int i = 0; i < args.Length; i++ ) {
                if( argument_patterns.IsMatch( args[ i ] ) ) {
                    matches = argument_patterns.Match( args[ i ] );
                    if( matches.Groups[ 1 ].Value != String.Empty ) {
                        test_flag = true;
                    } else if( matches.Groups[ 2 ].Value != String.Empty ) {
                        in_file = matches.Groups[ 2 ].Value;
                    } else if( matches.Groups[ 3 ].Value != String.Empty ) {
                        out_file = matches.Groups[ 4 ].Value;
                    }
                }
            }

            input = new StreamReader( in_file );
            if( test_flag ) {
                output = new StreamWriter( Console.OpenStandardOutput() );
                output.AutoFlush = true;
            } else if( out_file != String.Empty ) {
                output = new StreamWriter( out_file, false );
            } else {
                output = new StreamWriter( in_file.Replace( ".asm", ".hack" ), false );
            }
        }

        private static void findLabels( Parser parser, SymbolTable symbols ) {
            int ROM = 0;
            string symbol;

            while( parser.hasMoreCommands() ) {
                try {
                    parser.advance();
                } catch( EndOfStreamException ) {
                    break;
                }

                switch( parser.commandType() ) {
                    case Parser.L_COMMAND:
                        symbol = parser.symbol();
                        if( symbols.contains( symbol ) ) {
                            throw new InvalidOperationException( "Cannot redefine label: " + symbol );
                        }
                        symbols.addEntry( symbol, ROM );
                        break;
                    case Parser.A_COMMAND:
                    case Parser.C_COMMAND:
                        ROM++;
                        break;
                }
            }

            parser.reset();
        }

        private static void assemble( Parser parser, SymbolTable symbols, StreamWriter output ) {
            Code translator = new Code();
            int RAM = 16;

            while( parser.hasMoreCommands() ) {
                int value = 0;

                try {
                    parser.advance();
                } catch( EndOfStreamException ) {
                    break;
                }

                switch( parser.commandType() ) {
                    case Parser.A_COMMAND:
                        int address;
                        string symbol = parser.symbol();
                        bool isNumber = int.TryParse( symbol, out address );

                        if( !isNumber ) {
                            if( !symbols.contains( symbol ) ) {
                                symbols.addEntry( symbol, RAM++ );
                            }
                            address = symbols.getAddress( symbol );
                        }
                        value = address;
                        break;
                    case Parser.C_COMMAND:
                        value |= 7 << 13; // 1110000000000000 == 57344
                        value |= translator.comp( parser.comp() );
                        value |= translator.dest( parser.dest() );
                        value |= translator.jump( parser.jump() );
                        break;
                    case Parser.L_COMMAND:
                        continue;
                }

                string instruction = Convert.ToString( value, 2 ).PadLeft( 16, '0' );
                output.WriteLine( instruction );
            }

            parser.reset();
        }
    }
}
