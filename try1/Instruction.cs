using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace try1
{
    public class Instruction
    {
        public int rd; // rd <- rs + rt
        public int rs;
        public int rt;
        public int op; // add,sub,mult,div
        public int imm; //immediate number
        public int issueClock;
        public int executeClockBegin;
        public int executeClockEnd;
        public int writebackClock;
        public string name;
        //**** Class methods


        public Instruction()
        {
            rd = 0;
            rs = 0;
            rt = 0;
            op = 0;
            imm = 0;
            issueClock = 0;
            executeClockBegin = 0;
            executeClockEnd = 0;
            writebackClock = 0;
        }

        public Instruction(int RD, int RS, int RT, int OP)
        {
            rd = RD;
            rs = RS;
            rt = RT;
            op = OP;
            issueClock = 0;
            executeClockBegin = 0;
            executeClockEnd = 0;
            writebackClock = 0;
        }


    }
}
