    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Assembler {
    class Program {
        static void Main( string[] args ) {
            string input_filepath = args[ 0 ];
            string output_filepath;

            if( args.Length > 1 ) {
                output_filepath = args[ 1 ];
            } else {
                output_filepath = input_filepath.Replace( ".asm", ".hack" );
            }

            StreamReader file = new StreamReader( input_filepath );

            Parser parser = new Parser( file );
            SymbolTable symbols = new SymbolTable();

            findLabels( parser, symbols );

            StreamWriter output = new StreamWriter( output_filepath, false );

            assemble( parser, symbols, output );

            output.Close();
        }

        private static void findLabels( Parser parser, SymbolTable symbols ) {
            int ROM = 0;

            while( parser.hasMoreCommands() ) {
                try {
                    parser.advance();
                } catch( EndOfStreamException ) {
                    break;
                }

                switch( parser.commandType() ) {
                    case Parser.L_COMMAND:
                        if( symbols.contains( parser.symbol() ) ) {
                            //throw redifined label exception
                        }
                        symbols.addEntry( parser.symbol(), ROM );
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
