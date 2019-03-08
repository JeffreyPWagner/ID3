using System;
using System.IO;

namespace IterativeDichotomiser {
    class Program {
        static void Main(string[] args) {
            ID3 myID3 = new ID3();
            myID3.readFile();
        }
    }
}
