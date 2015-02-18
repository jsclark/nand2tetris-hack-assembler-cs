using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    class Code {
        public Code() {
        }

        private Dictionary<string, int> comp_map = new Dictionary<string, int> {
            { "0",      42 }, // 101010
            { "1",      63 }, // 111111
            { "-1",     58 }, // 111010
            { "D",      12 }, // 001100
            { "A",      48 }, // 110000
            { "!D",     13 }, // 001101
            { "!A",     49 }, // 110001
            { "-D",     15 }, // 001111
            { "-A",     51 }, // 110011
            { "D+1",    31 }, // 011111
            { "A+1",    55 }, // 110111
            { "D-1",    14 }, // 001110
            { "A-1",    50 }, // 110010
            { "D+A",     2 }, // 000010
            { "D-A",    19 }, // 010011
            { "A-D",     7 }, // 000111
            { "D&A",     0 }, // 000000
            { "D|A",    21 }, // 010101
        };

        private Dictionary<string, int> jump_map = new Dictionary<string, int> {
            { "JGT",    1 }, // 001
            { "JEQ",    2 }, // 010
            { "JGE",    3 }, // 011
            { "JLT",    4 }, // 100
            { "JNE",    5 }, // 101
            { "JLE",    6 }, // 110
            { "JMP",    7 }, // 111
        };

        public int dest( string dest ) {
            int value = 0;

            if( dest != String.Empty ) {
                if( dest.IndexOf( "A" ) != -1 ) {
                    value |= 1 << 2;
                }

                if( dest.IndexOf( "D" ) != -1 ) {
                    value |= 1 << 1;
                }

                if( dest.IndexOf( "M" ) != -1 ) {
                    value |= 1;
                }
            }

            return value << 3;
        }

        public int comp( string comp ) {
            int value = 0;

            if( comp != String.Empty ) {
                if( comp.IndexOf( "M" ) != -1 ) {
                    comp = comp.Replace( "M", "A" );
                    value |= 64;
                }

                if( !comp_map.ContainsKey( comp ) ) {
                    throw new ArgumentOutOfRangeException();
                }

                value |= comp_map[ comp ];
            }

            return value << 6;
        }

        public int jump( string jump ) {
            int value = 0;

            if( jump != String.Empty ) {
                if( !jump_map.ContainsKey( jump ) ) {
                    throw new ArgumentOutOfRangeException();
                }

                value |= jump_map[ jump ];
            }

            return value;
        }
    }
}
