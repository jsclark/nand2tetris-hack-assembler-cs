using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

namespace Assembler {
    class Parser {
        public const int A_COMMAND = 1;
        public const int C_COMMAND = 2;
        public const int L_COMMAND = 3;

        private StreamReader input;
        private string line;

        private int _command;
        private string _symbol;
        private string _dest;
        private string _comp;
        private string _jump;

        private static Regex space = new Regex( @"\s+" );
        private static Regex comment = new Regex( @"//.*" );
        private static Regex c_pattern = new Regex( @"^(([AMD]+)=)?([AMD]?[+&-|!]?[01AMD])(;(J(GT|EQ|GE|LT|NE|LE|MP)))?$" );
        private static Regex a_pattern = new Regex( @"^@((\d+)|([a-zA-Z_.$:].*))$" );
        private static Regex l_pattern = new Regex( @"^\(([a-zA-Z_.$:].*)\)$" );


        public Parser( StreamReader input ) {
            this.input = input;
        }

        public bool hasMoreCommands() {
            return !( input.EndOfStream );
        }

        public void reset() {
            line = null;
            input.BaseStream.Position = 0;
            input.DiscardBufferedData();
        }

        public void advance() {
            // this can happen if the last line of a file is whitespace or a comment
            if( !hasMoreCommands() ) {
                throw new EndOfStreamException();
            }

            line = input.ReadLine();
            // remove all whitespace
            line = space.Replace( line, String.Empty );
            // remove all comments
            line = comment.Replace( line, String.Empty );

            if( line == String.Empty ) {
                advance();
            } else {
                parse();
            }
        }

        private void parse() {
            if( a_pattern.IsMatch( line ) ) {
                _command = A_COMMAND;
                parseA();
            } else if( l_pattern.IsMatch( line ) ) {
                _command = L_COMMAND;
                parseL();
            } else if( c_pattern.IsMatch( line ) ) {
                _command = C_COMMAND;
                parseC();
            } else {
                // throw invalid instruction
            }
        }

        private void parseA() {
            Match matches = a_pattern.Match( line );
            _symbol = matches.Groups[ 1 ].Value;
            _dest = _comp = _jump = String.Empty;
        }

        private void parseL() {
            Match matches = l_pattern.Match( line );
            _symbol = matches.Groups[ 1 ].Value;
            _dest = _comp = _jump = String.Empty;
        }

        private void parseC() {
            _symbol = String.Empty;
            Match matches = c_pattern.Match( line );

            _dest = matches.Groups[ 2 ].Value;
            _comp = matches.Groups[ 3 ].Value;
            _jump = matches.Groups[ 5 ].Value;
        }

        public int commandType() {
            return _command;
        }

        public string symbol() {
            if( _command == C_COMMAND ) {
                throw new InvalidOperationException();
            }
            return _symbol;
        }

        public string dest() {
            if( _command != C_COMMAND ) {
                throw new InvalidOperationException(); 
            }
            return _dest;
        }

        public string comp() {
            if( _command != C_COMMAND ) {
                throw new InvalidOperationException();
            }
            return _comp;
        }

        public string jump() {
            if( _command != C_COMMAND ) {
                throw new InvalidOperationException();
            }
            return _jump;
        }

    }
}
