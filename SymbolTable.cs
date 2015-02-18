using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler {
    class SymbolTable {

        private Dictionary<string,int> symbols = new Dictionary<string,int>{
            { "SP",     0x0000 },
            { "LCL",    0x0001 },
            { "ARG",    0x0002 },
            { "THIS",   0x0003 },
            { "THAT",   0x0004 },
            { "SCREEN", 0x4000 },
            { "KBD",    0x6000 },
        };

        public SymbolTable() {
            // add registers
            for( int i = 0; i < 16; i++ ) {
                symbols.Add( "R" + i, i );
            }
        }

        public void addEntry( string symbol, int address ) {
            symbols.Add( symbol, address );
        }

        public bool contains( string symbol ) {
            return symbols.ContainsKey( symbol );
        }

        public int getAddress( string symbol ) {
            if( !contains( symbol ) ) {
                // throw new UndefinedSymbol
            }
            return symbols[ symbol ];
        }
    }
}
