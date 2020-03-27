using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace try1
{
    public partial class Form1 : Form
    {
        //**** Define Architecture
        public bool run = true;
        public bool pause_or_continue = true;
        //readfile
        public string line;
        // NUMBER OF RESERVATION STATIONS
        public int Num_LOAD_RS = 2;
        public int Num_ADD_RS = 3;
        public int Num_MULT_RS = 2;
        // Opcode Values
        const int AddOp = 0;
        const int SubOp = 1;
        const int MultOp = 2;
        const int DivOp = 3;
        const int LoadOp = 4;
        // RESERVATION STATION LATENCY
        public int ADD_Lat = 3;
        public int MULT_Lat = 11;
        public int DIV_Lat = 41;
        public int LOAD_Lat = 2;
        // Datapath Latency
        const int ISSUE_Lat = 1;
        const int WRITEBACK_Lat = 1;
        //**** Do not edit these constants
        // Global Clock
        int Clock = 0;
        // used to check if INST == WRITEBACKS to end program
        bool Done = true;
        int Total_WRITEBACKS = 0;
        // Counter for current instruction to issue
        int currentInst_ISSUE = 0;
        // Temporary fix for errors due to RS names being numbers
        // -> errors with REG/RS/REGSTATUS == zero
        const int ZERO_REG = 5000;
        const int RegStatusEmpty = 1000;
        const int OperandAvailable = 1001;
        const int OperandInit = 1002;
        //Add
        private Instruction[] Inst = new Instruction[100];
        private int Inst_size;
        ReservationStation LOAD1, LOAD2, ADD1, ADD2, ADD3, MULT1, MULT2;
        private ReservationStation[] ResStation = new ReservationStation[100];
        private int ResStation_size;
        RegisterStatus F0, F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12;
        string[] Register_Memory = new string[12] { "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000" };

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }


        public void SetResStation(int num_load, int num_add, int num_mult,int load_lat, int add_lat, int mult_lat,int div_lat)
        {
             Num_LOAD_RS = num_load;
             Num_ADD_RS = num_add;
             Num_MULT_RS = num_mult;
             LOAD_Lat = load_lat;
             ADD_Lat = add_lat;
             MULT_Lat = mult_lat;
             DIV_Lat = div_lat;
            for (int i = 0; i < Num_LOAD_RS; i++)
             {
                ReservationStation LOAD = new ReservationStation(LoadOp, OperandInit, "LOAD" + (i + 1).ToString());
                ResStation[i] = LOAD;
             }
             for (int i = 0; i < Num_ADD_RS; i++)
             {
                ReservationStation ADD = new ReservationStation(AddOp, OperandInit, "ADD" + (i + 1).ToString());
                ResStation[i + Num_LOAD_RS] = ADD;
             }
            for (int i = 0; i < Num_MULT_RS; i++)
            {
                ReservationStation MULT = new ReservationStation(MultOp, OperandInit, "MULT" + (i + 1).ToString());
                ResStation[i + Num_LOAD_RS + Num_ADD_RS] = MULT;
            }
            ResStation_size = Num_LOAD_RS + Num_ADD_RS + Num_MULT_RS;
            //Input reservation station architecture
            Init_Instruction();
            Init_Regiser();
            Init_Clock();
            Update_Clock_Board1();
            Update_Clock_Board();
            Update_Instruction_Board();
            Update_ReservationStation_Board();
            Update_Register_Board();


            //**** Define Architecture
            run = true;
            pause_or_continue = true;
            Clock = 0;
            // used to check if INST == WRITEBACKS to end program
            Done = true;
            Total_WRITEBACKS = 0;
            // Counter for current instruction to issue
            currentInst_ISSUE = 0;
            Register_Memory = new string[12] { "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000" };
        }

        //Save
        private void Button8_Click(object sender, EventArgs e)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(@"\\Mac\Home\Desktop\instruction1.txt"))
            {
                sw.WriteLine("      Instruction       IS    Ex-Begin    Ex-end    WB");
                for(int i=0;i<Inst_size;i++)
                {
                    string Instruction_output1, Instruction_output2, Instruction_output3, Instruction_output4, Instruction_output5;
                    Instruction_output2 = Inst[i].issueClock.ToString();
                    Instruction_output3 = Inst[i].executeClockBegin.ToString();
                    Instruction_output4 = Inst[i].executeClockEnd.ToString();
                    Instruction_output5 = Inst[i].writebackClock.ToString();
                    int j = 0;
                    int k =13 - Inst[i].issueClock.ToString().Length;
                    if (Inst[i].op == LoadOp)
                    {
                        Instruction_output1 = Inst[i].name + " F" + Inst[i].rd + "," + Inst[i].imm + "(R" + Inst[i].rs + ") ";
                        j = 25 - Instruction_output1.Length;
                    }
                    else
                    {
                        Instruction_output1 = Inst[i].name + " F" + Inst[i].rd + ",F" + Inst[i].rs + ",F" + Inst[i].rt;
                        j = 25 - Instruction_output1.Length;
                    }
                    sw.WriteLine(Instruction_output1 + string.Empty.PadRight(j, ' ') + Instruction_output2 + string.Empty.PadRight(k, ' ')+
                        Instruction_output3 + string.Empty.PadRight(k, ' ') + Instruction_output4 + string.Empty.PadRight(k, ' ') + Instruction_output5);
                }
                sw.WriteLine(" --------------------------------------------------------------------- ");
                sw.WriteLine("   Name    " + "     Busy   " + "     Op      " + "      Vj      " + "     Vk       " + "    Qj   " + "     Qk    " + "   A      ");
                for(int i = 0; i < ResStation_size; i++)
                {
                    string str1 = " ";
                    string str2, str3, str4;

                    if (ResStation[i].busy)
                    {
                        switch (ResStation[i].op)
                        {
                            case 0:
                                str1 = "ADD.D";
                                break;
                            case 1:
                                str1 = "SUB.D";
                                break;
                            case 2:
                                str1 = "MUL.D";
                                break;
                            case 3:
                                str1 = "DIV.D";
                                break;
                            case 4:
                                str1 = "L.D";
                                break;
                            default:
                                break;
                        }
                        if (ResStation[i].Qj == OperandAvailable)
                            str2 = ResStation[i].Vj.ToString();
                        else str2 = "        ";
                        if (ResStation[i].Qk == OperandAvailable)
                            str3 = ResStation[i].Vk.ToString();
                        else str3 = "        ";
                    }
                    else
                    {
                        str1 = "          ";
                        str2 = "           ";
                        str3 = "           ";
                    }

                    if (ResStation[i].Qj == OperandInit) str4 = "            ";
                    else if (ResStation[i].Qj == OperandAvailable) str4 = "        ";
                    else str3 = ResStation[ResStation[i].Qj].name;

                    if (ResStation[i].Qk == OperandInit) str4 = "           ";
                    else if (ResStation[i].Qk == OperandAvailable) str4 = "           ";
                    else str4 = ResStation[ResStation[i].Qk].name; 


                    sw.WriteLine(ResStation[i].name + "     " + ResStation[i].busy.ToString() +  "      "+ str1 + "       " + str2 + "      " +  str3 + "     " + str4);


                }
                sw.WriteLine(" --------------------------------------------------------------------- ");
                string final = " ";
                for (int i = 0; i < RegisterStatus_size; i++)
                {
                    if (RegisterStatus[i].Qi == RegStatusEmpty) final += "     ";
                    else final += ResStation[RegisterStatus[i].Qi].name + "(F" + i + ")";
                }
                sw.WriteLine("    Qi     " + final);
            }

        }

        private void TableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        //Reset
        private void Button5_Click_1(object sender, EventArgs e)
        {
            Init_Instruction();
            Init_ReservationStation();
            Init_Regiser();
            Init_Clock();
            Update_Clock_Board1();
            Update_Clock_Board();
            Update_Instruction_Board();
            Update_ReservationStation_Board();
            Update_Register_Board();


        //**** Define Architecture
            run = true;
            pause_or_continue = true;
            Clock = 0;
        // used to check if INST == WRITEBACKS to end program
            Done = true;
            Total_WRITEBACKS = 0;
        // Counter for current instruction to issue
            currentInst_ISSUE = 0;
            Register_Memory = new string[12] { "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000" };
        }


        //pause
    private void Button4_Click(object sender, EventArgs e)
        {
            pause_or_continue = !pause_or_continue;
            Button2_Click(sender, e);
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.parent = this;
            form2.Show(this);
            //Hide();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            run = false;
        }

        private void Update_Clock_Board1()
        {
            textBox1.Text = "Clock: " + Clock.ToString();
        }



        public bool IsAllBufferFree()
        {
            for(int i = 0; i < ResStation_size; i++)
            {
                if (ResStation[i].busy) return false;
            }
            return true;
        }

        //auto
        private void Button2_Click(object sender, EventArgs e)
        {
            run = true;
            while (run)
            {
                if (!pause_or_continue) break;
                //缺少一个执行完毕可以停下来
                // if (instructionUnit.GetCurrentInstructions().Length == 0) break;
                //如果每个保留站都是空的，则完成
                System.Threading.Thread.Sleep(1000);
                Clock++;
                RunOneCycle();
                Update_Clock_Board1();
                //TODO:每个保留站检查是否清空
                Application.DoEvents();
                if (IsAllBufferFree()) break;
            }
        }

        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private RegisterStatus[] RegisterStatus;
        private int RegisterStatus_size;
        private int[] Register;
        //#######################################################################



        public Form1()
        {
            InitializeComponent();
        }


        public void Init_Instruction()
        {
            read_from_file();
        }

        public void Init_ReservationStation()
        {
            Num_LOAD_RS = 2;
            Num_ADD_RS = 3;
            Num_MULT_RS = 2;
            ADD_Lat = 3;
            MULT_Lat = 11;
            DIV_Lat = 41;
            LOAD_Lat = 2;
            LOAD1 = new ReservationStation(LoadOp, OperandInit, "LOAD1");
            LOAD2 = new ReservationStation(LoadOp, OperandInit, "LOAD2");
            ADD1 = new ReservationStation(AddOp, OperandInit, "ADD1");
            ADD2 = new ReservationStation(AddOp, OperandInit, "ADD2");
            ADD3 = new ReservationStation(AddOp, OperandInit, "ADD3");
            MULT1 = new ReservationStation(MultOp, OperandInit, "MULT1");
            MULT2 = new ReservationStation(MultOp, OperandInit, "MULT2");
            ResStation_size = 7;
            ResStation = new ReservationStation[7] { LOAD1, LOAD2, ADD1, ADD2, ADD3, MULT1, MULT2 };


        }

        public void Init_Regiser()
        {
            //Initialize register statue ovjects
            F0 = new RegisterStatus(RegStatusEmpty);
            F1 = new RegisterStatus(RegStatusEmpty);
            F2 = new RegisterStatus(RegStatusEmpty);
            F3 = new RegisterStatus(RegStatusEmpty);
            F4 = new RegisterStatus(RegStatusEmpty);
            F5 = new RegisterStatus(RegStatusEmpty);
            F6 = new RegisterStatus(RegStatusEmpty);
            F7 = new RegisterStatus(RegStatusEmpty);
            F8 = new RegisterStatus(RegStatusEmpty);
            F9 = new RegisterStatus(RegStatusEmpty);
            F10 = new RegisterStatus(RegStatusEmpty);
            F11 = new RegisterStatus(RegStatusEmpty);
            F12 = new RegisterStatus(RegStatusEmpty);
            RegisterStatus_size = 13;
            RegisterStatus = new RegisterStatus[13] { F0, F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12 };

            /*   //Initialize regitser file vector
              Register = new int[13] { ZERO_REG, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };*/
        }

        public void Init_Clock()
        {
            textBox10.Text = "Clock:0 ";
        }


        private void read_from_file()
        {
            int mark = 0;
            System.IO.StreamReader file = new System.IO.StreamReader(@"\\Mac\Home\Desktop\instruction.txt");
            while ((line = file.ReadLine()) != null)
            {
                mark = 0;
                int flag = 0;
                Instruction I0 = new Instruction();
                for (int i = 0; i < line.Length-1; i++)
                {
                    if (flag == 0)
                    {
                        mark += line[i] - 48;
                        if (line[i + 1] == ' '|| line[i + 1] == '\t') flag = 1;
                        else mark = mark * 10;
                    } 
                    else if (flag == 1&&line[i]!=' '&&line[i]!='\t')
                    {
                        I0.name += line[i];
                        if (line[i + 1] == ' ' || line[i + 1] == '\t')
                        {
                            switch (I0.name)
                            {
                                case "ADD.D":
                                    I0.op = AddOp;
                                    break;
                                case "SUB.D":
                                    I0.op = SubOp;
                                    break;
                                case "MUL.D":
                                    I0.op = MultOp;
                                    break;
                                case "DIV.D":
                                    I0.op = DivOp;
                                    break;
                                case "L.D":
                                    I0.op = LoadOp;
                                    break;
                            }
                            flag = 2;
                            I0.name += " ";

                        }   
                    }
                    else if (flag == 2 && line[i] != ' ' && line[i] != '\t'){
                        if (I0.name == "L.D ") 
                        {
                            if (flag == 2 && char.IsDigit(line[i]))
                            {
                                I0.rd += line[i] - 48;
                                if (line[i + 1] != ',') I0.rd = I0.rd * 10;
                                else flag = 3;
                            }
                        }
                        else if (flag == 2 && char.IsDigit(line[i]))
                        {
                            I0.rd += line[i] - 48;
                            if (line[i + 1] != ',') I0.rd = I0.rd * 10;
                            else flag = 3;
                        }
                    }
                    else if (flag == 3 && char.IsDigit(line[i]))
                    {
                        if(I0.name == "L.D ")
                        {
                            I0.imm += line[i] - 48;
                            if (line[i + 1] != '(') I0.imm = I0.imm * 10;
                            else flag = 4;
                        }
                        else
                        {
                            I0.rs += line[i] - 48;
                            if (line[i + 1] != ',') I0.rs = I0.rs * 10;
                            else flag = 4;
                        }
                    }
                    else if (flag == 4)
                    {
                        if (I0.name == "L.D ")
                        {
                            if (char.IsDigit(line[i]))
                            {
                                I0.rs += line[i] - 48;
                                if (line[i + 1] != ')') I0.rs = I0.rs * 10;
                                else
                                {
                                    flag = 5;
                                    break;
                                }
                            }
                        }
                        else
                        {

                            if (i + 1 == line.Length - 1)
                            {
                                if (char.IsDigit(line[i]))
                                {
                                    I0.rt += line[i] - 48;
                                    I0.rt = I0.rt * 10;
                                    I0.rt += line[i + 1] - 48;
                                    break;
                                }
                                else I0.rt = line[i + 1] - 48;
                            }
                            else continue;
                        }
                    }
                    Inst[mark - 1] = I0;
                }
            }
            Inst_size = mark;
        }


        /*string[] sArray = line.Split(new char[3] { '\t','\n','\r' }, StringSplitOptions.RemoveEmptyEntries);
         foreach (string i in sArray)
         {
             bool IsNumber = int.TryParse(i, out digit);
             if (!IsNumber)
             {
                 switch (i)
                 {
                     case "ADD.D":
                         I0.op = AddOp;
                         I0.name = i;
                         break;
                     case "SUB.D":
                         I0.op = SubOp;
                         I0.name = i;
                         break;
                     case "MUL.D":
                         I0.op = MultOp;
                         I0.name = i;
                         break;
                     case "DIV.D":
                         I0.op = DivOp;
                         I0.name = i;
                         break;
                     case "L.D":
                         I0.op = LoadOp;
                         I0.name = i;
                         flag = 1;
                         break;
                     default:
                         flag = 2;
                         break;


                 }
             }
             else mark = digit;


             //for load: rt<-imm+rs;
             if (flag == 1)
             {
                 textBox2.Text = i;
                 int flag1 = 0;
                 foreach (char j in i)
                 {
                     if (char.IsDigit(j))
                     {
                         digit = (int)j - 48;

                         if (flag1 == 0)
                         {
                             I0.rt = digit;
                             flag1 = 1;
                         }
                         else if (flag1 == 1)
                         {
                             I0.imm = digit;
                             flag1 = 2;
                         }
                         else if (flag1 == 2)
                         {
                             I0.rs = digit;
                             flag1 = 2;
                             break;
                         }
                     }

                 }
                 Inst[mark - 1] = I0;
             }

             //for others
             if (flag == 2)
             {
                 //textBox2.Text = i;
                 //string str = "F6,F8,F2";
                 //string[] sArray1 = str.Split(new char[2] { 'F', ',' },StringSplitOptions.RemoveEmptyEntries);
                 //string[] sArray1 = i.Split(new char[2] { 'F', ',' },StringSplitOptions.RemoveEmptyEntries);
                 //textBox2.Text = sArray1[1];

                 int flag1 = 0;
                 foreach (char j in i)
                 {
                     if (char.IsDigit(j))
                     {
                         digit = (int)j - 48;

                         if (flag1 == 0)
                         {
                             I0.rd = digit;
                             flag1 = 1;
                         }
                         else if (flag1 == 1)
                         {
                             I0.rs = digit;
                             flag1 = 2;
                         }
                         else if (flag1 == 2)
                         {
                             I0.rt = digit;
                             flag1 = 2;
                             break;
                         }
                     }

                 }*/




        private void Form1_Load(object sender, EventArgs e)
        {
            Init_Instruction();
            Init_ReservationStation();
            Init_Regiser();
            Init_Clock();
        }

        private void Label1_Click(object sender, EventArgs e)
        {
            label1.Text = "Instruction  ";
        }

        private void Panel2_Paint(object sender, PaintEventArgs e)
        {
            label1.Text = "Instruction  ";
        }

        private void TableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
            label1.Text = "Instuction  ";
            label2.Text = "State";
 
        }



        private int ISSUE(Instruction[] INST, ReservationStation[] RESSTATION, RegisterStatus[] REGSTATUS)
        {
            // checi if spot in given reservation station is available
            // Latency of 1 if issued
            //**** check if spot in given reservation station is available
            int r = 0;
            bool rsFree = false;
            // r is the current instruction to be issued's operation
            // code(add,sub,mult,div)
            // If all instructions have been issued then stop issueing
            // for rest of program
            if (currentInst_ISSUE >= Inst_size)//实现思想：让currentInst_ISSUE那行的文字更新
                return 0;
            r = INST[currentInst_ISSUE].op;
            // determine if there is an open RS of r type. if yes
            // -> r = that open spot.
            // Boundry's of given RS
            int RsLoadStart = Num_LOAD_RS - Num_LOAD_RS;
            int RsLoadEnd = Num_LOAD_RS;
            int RSAddStart = Num_LOAD_RS;
            int RSAddEnd = Num_LOAD_RS + Num_ADD_RS;
            int RSSubStart = Num_LOAD_RS;
            int RSSubEnd = Num_LOAD_RS + Num_ADD_RS;
            int RSMulStart = Num_LOAD_RS + Num_ADD_RS;
            int RSMulEnd =  Num_LOAD_RS + Num_ADD_RS + Num_MULT_RS;
            switch (r)
            {
                case LoadOp:
                    for(int i = RsLoadStart; i < RsLoadEnd; i++)
                    {
                        if (!RESSTATION[i].busy)
                        {
                            r = i;
                            currentInst_ISSUE++;
                            RESSTATION[i].op = LoadOp;
                            rsFree = true;
                            break;
                        }
                    }
                    if (!rsFree)
                        return 1;
                    break;
                case AddOp:
                    for (int i = RSAddStart; i < RSAddEnd; i++)
                    {
                        if (!RESSTATION[i].busy)
                        {
                            r = i;
                            currentInst_ISSUE++;
                            RESSTATION[i].op = AddOp;
                            rsFree = true;
                            break;
                        }
                    }
                    // if instruction is not issued because no
                    // reservation stations are free exit ISSUE
                    // Init is not necessary if instruction not issued
                    if (!rsFree)
                        return 1;
                    break;
                case SubOp:
                    for (int i = RSSubStart; i < RSSubEnd; i++)
                    {
                        if (!RESSTATION[i].busy)
                        {
                            r = i;
                            currentInst_ISSUE++;
                            RESSTATION[i].op = SubOp;
                            rsFree = true;
                            break;
                        }
                    }
                    if (!rsFree)
                        return 1;
                    break;
                case MultOp:
                    for (int i = RSMulStart; i < RSMulEnd; i++)
                    {
                        if (!RESSTATION[i].busy)
                        {
                            r = i;
                            currentInst_ISSUE++;
                            RESSTATION[i].op = MultOp;
                            rsFree = true;
                            break;
                        }
                    }
                    if (!rsFree)
                        return 1;
                    break;
                case DivOp:
                    for (int i = RSMulStart; i < RSMulEnd; i++)
                    {
                        if (!RESSTATION[i].busy)
                        {
                            r = i;
                            currentInst_ISSUE++;
                            RESSTATION[i].op = DivOp;
                            rsFree = true;
                            break;
                        }
                    }
                    if (!rsFree)
                        return 1;
                    break;
                default:
                    break;
            }
            //**** Initialize characteristics of issued instruction
            // if operand rs is available -> set value of operand
            // (Vj) to given register value
            // else point operand to the reservation station (Qj)
            // that will give the operand value
            // NOTE: since currentInst was in incremented we must
            // do currentINST_ISSUE-1
            if (INST[currentInst_ISSUE - 1].op != LoadOp)
            {
                if (REGSTATUS[INST[currentInst_ISSUE - 1].rs].Qi == RegStatusEmpty)
                {
                    RESSTATION[r].Vj += "Regs[F" + INST[currentInst_ISSUE - 1].rs + "]";
                    RESSTATION[r].Qj = OperandAvailable;
                }
                else
                {
                    RESSTATION[r].Qj = REGSTATUS[INST[currentInst_ISSUE - 1].rs].Qi;
                }
                // if operand rt is available -> set value of
                // operand (Vk) to given register value
                // else point operand to the reservation station
                // (Qk) that will give the operand value
                if (REGSTATUS[INST[currentInst_ISSUE - 1].rt].Qi == RegStatusEmpty)
                {
                    RESSTATION[r].Vk = "Regs[F" + INST[currentInst_ISSUE - 1].rt + "]";
                    RESSTATION[r].Qk = OperandAvailable;
                }
                else
                {
                    RESSTATION[r].Qk = REGSTATUS[INST[currentInst_ISSUE - 1].rt].Qi;
                }
            }
            else
            {
                RESSTATION[r].A += INST[currentInst_ISSUE - 1].imm.ToString() ;
            }
                // given reservation station is now busy
                // until write back stage is completed.
                RESSTATION[r].busy = true;
                RESSTATION[r].ISSUE_Lat = 0;
                // set reservation station instuction
                // number == current instruction
                RESSTATION[r].instNum = currentInst_ISSUE - 1;
                // set clock cycle for issue time
                INST[currentInst_ISSUE - 1].issueClock = Clock;
                // The register status Qi is set to the current
                // instructions reservation station location r
                REGSTATUS[INST[currentInst_ISSUE - 1].rd].Qi = r;
            return 2;
        }

        private void EXECUTE(Instruction[] INST, ReservationStation[] RESSTATION, RegisterStatus[] REGSTATUS)
        {
            // check each reservation station to see
            // if both operands are ready
            // The current reservation station is r
            for (int r = 0; r < ResStation_size; r++)
            {
                // if both operands are available then
                // execute given instructions operation
                // and set resultReady flag to true so that
                // result can be written back to CDB
                // first check if instruction has been issued
                if (RESSTATION[r].busy == true)
                {
                    // second check if the ISSUE latency clock cycle has happened
                    if (RESSTATION[r].ISSUE_Lat >= ISSUE_Lat)
                    {
                        // third check if both operands are available
                        // for load

                        //for the other 
                        if ((RESSTATION[r].Qj == OperandAvailable &&
                           RESSTATION[r].Qk == OperandAvailable)||(RESSTATION[r].op == LoadOp))
                        {
                            // Set clock cycle when execution begins
                            if (INST[RESSTATION[r].instNum].executeClockBegin == 0)
                            {
                                INST[RESSTATION[r].instNum].executeClockBegin = Clock;
                            }
                            // when execution starts we must wait the given
                            // latency number of clock cycles before making result
                            // available to WriteBack
                            // Delay: Switch(INST.op)
                            //        case(add):     clock += 4;
                            //        case(mult):     clock += 12;
                            //        case(div):    clock += 38;
                            RESSTATION[r].lat++;
                            switch (RESSTATION[r].op)
                            {
                                case (AddOp):
                                    if (RESSTATION[r].lat == ADD_Lat)
                                    {
                                        //RESSTATION[r].result = RESSTATION[r].Vj + RESSTATION[r].Vk;
                                        // Result is ready to be writenback
                                        RESSTATION[r].resultReady = true;
                                        RESSTATION[r].lat = 0;
                                        // Set clock cycle when execution ends
                                        INST[RESSTATION[r].instNum].executeClockEnd = Clock;
                                        // reset ISSUE latency for RS
                                        RESSTATION[r].ISSUE_Lat = 0;
                                        Register_Memory[INST[RESSTATION[r].instNum].rd] = "000";
                                    }
                                    break;
                                case (SubOp):
                                    if (RESSTATION[r].lat == ADD_Lat)
                                    {
                                        //RESSTATION[r].result = RESSTATION[r].Vj - RESSTATION[r].Vk;
                                        RESSTATION[r].resultReady = true;
                                        RESSTATION[r].lat = 0;
                                        // Set clock cycle when execution ends
                                        INST[RESSTATION[r].instNum].executeClockEnd = Clock;
                                        // reset ISSUE latency for RS
                                        RESSTATION[r].ISSUE_Lat = 0;
                                        Register_Memory[INST[RESSTATION[r].instNum].rd] = "000";
                                    }
                                    break;
                                case (MultOp):
                                    if (RESSTATION[r].lat == MULT_Lat)
                                    {
                                        //RESSTATION[r].result = RESSTATION[r].Vj * RESSTATION[r].Vk;
                                        RESSTATION[r].resultReady = true;
                                        RESSTATION[r].lat = 0;
                                        // Set clock cycle when execution ends
                                        INST[RESSTATION[r].instNum].executeClockEnd = Clock;
                                        // reset ISSUE latency for RS
                                        RESSTATION[r].ISSUE_Lat = 0;
                                        Register_Memory[INST[RESSTATION[r].instNum].rd] = "000";
                                    }
                                    break;
                                case (DivOp):
                                    if (RESSTATION[r].lat == DIV_Lat)
                                    {
                                        //RESSTATION[r].result = RESSTATION[r].Vj / RESSTATION[r].Vk;
                                        RESSTATION[r].resultReady = true;
                                        RESSTATION[r].lat = 0;
                                        // Set clock cycle when execution ends
                                        INST[RESSTATION[r].instNum].executeClockEnd = Clock;
                                        // reset ISSUE latency for RS
                                        RESSTATION[r].ISSUE_Lat = 0;
                                        Register_Memory[INST[RESSTATION[r].instNum].rd] = "000";
                                    }
                                    break;
                                case (LoadOp):
                                    if (RESSTATION[r].lat == LOAD_Lat)
                                    {
                                        RESSTATION[r].resultReady = true;
                                        RESSTATION[r].lat = 0;
                                        // Set clock cycle when execution ends
                                        INST[RESSTATION[r].instNum].executeClockEnd = Clock;
                                        RESSTATION[r].A = INST[RESSTATION[r].instNum].imm.ToString() + "+Regs[R" + INST[RESSTATION[r].instNum].rs + "]";
                                        Register_Memory[INST[RESSTATION[r].instNum].rd] = "Mem[" + RESSTATION[r].A + "]";
                                        // reset ISSUE latency for RS
                                        RESSTATION[r].ISSUE_Lat = 0;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    else // Execute is not ready until one cycle latency of ISSUE
                        RESSTATION[r].ISSUE_Lat++;
                }

            }
        }

        private void WRITEBACK(Instruction[] INST, ReservationStation[] RESSTATION, RegisterStatus[] REGSTATUS)
        {
            // Check each reservation station to see
            // if operational delay is done -> result is ready
            for (int r = 0; r < ResStation_size; r++)
            {
                // if result ready write back to CDB
                // -> Register,and reservation stations
                if (RESSTATION[r].resultReady)
                {
                    // Before Writeback is available there
                    // must be a 1 cycle WB delay
                    if (RESSTATION[r].WRITEBACK_Lat == WRITEBACK_Lat)
                    {
                        // set clock cycle when write back occured.
                        // (Must add one because increment happens after loop)
                        if (INST[RESSTATION[r].instNum].writebackClock == 0)
                        {
                            INST[RESSTATION[r].instNum].writebackClock = Clock;
                        }
                            
                        // Check if any registers (via the registerStatus)
                        // are waiting for current r result
                        for (int x = 0; x < RegisterStatus_size; x++)
                        {
                            // if RegisterStatus points to the given
                            // reservation station r set that register[x]
                            // equal to executed result
                            if (REGSTATUS[x].Qi == r)
                            {
                                // Write back to Registers
                                //REG[x] = RESSTATION[r].result;
                                REGSTATUS[x].Qi = RegStatusEmpty;
                            }
                        }
                        // Check if any reservation stations are
                        // waiting for current r result
                        for (int y = 0; y < ResStation_size; y++)
                        {
                            // check if any reservation stations are
                            // waiting for the given result as an operand
                            // Write back to reservation stations
                            // Given RS is not longer waiting for this
                            // operand value
                            if (RESSTATION[y].Qj == r)
                            {
                                if (Register_Memory[INST[RESSTATION[r].instNum].rd] != "000") RESSTATION[y].Vj = Register_Memory[INST[RESSTATION[r].instNum].rd];
                                else RESSTATION[y].Vj = "Regs[F" + Inst[RESSTATION[r].instNum].rd + "]";  
                                RESSTATION[y].Qj = OperandAvailable;
                            }
                            if (RESSTATION[y].Qk == r)
                            {
                                if (Register_Memory[INST[RESSTATION[r].instNum].rd] != "000") RESSTATION[y].Vk = Register_Memory[INST[RESSTATION[r].instNum].rd];
                                else RESSTATION[y].Vk = "Regs[F" + Inst[RESSTATION[r].instNum].rd + "]";
                                RESSTATION[y].Qk = OperandAvailable;
                            }
                        }
                        // The given reservation station can
                        // now be used again
                        // Reset RS paramaters
                        RESSTATION[r].resultReady = false;
                        RESSTATION[r].busy = false;
                        RESSTATION[r].Qj = OperandInit;
                        RESSTATION[r].Qk = OperandInit;
                        RESSTATION[r].WRITEBACK_Lat = 0;
                        Total_WRITEBACKS++;
                    }
                    else
                        RESSTATION[r].WRITEBACK_Lat++;
                }
            }
        }

        private void Update_Clock_Board()
        {
            textBox10.Text = "Clock: " + Clock.ToString();
        }

        private void Update_Instruction_Board()
        {
            listView1.Clear();
            listView1.View = View.Details;

            ColumnHeader clh;
            clh = new ColumnHeader();
            clh.Text = "   Instruction     ";
            listView1.Columns.Add(clh);
            clh = new ColumnHeader();
            clh.Text = "Issue  ";
            listView1.Columns.Add(clh);
            clh = new ColumnHeader();
            clh.Text = "Exe-begin ";
            listView1.Columns.Add(clh);
            clh = new ColumnHeader();
            clh.Text = "Exe-end ";
            listView1.Columns.Add(clh);
            clh = new ColumnHeader();
            clh.Text = "WriteBack ";
            listView1.Columns.Add(clh);
            listView1.Columns[0].Width = -2;
            listView1.Columns[1].Width = -2;
            listView1.Columns[2].Width = -2;
            listView1.Columns[3].Width = -2;
            listView1.Columns[4].Width = -2;

            //下面添加信息

            for (int i = 0; i < Inst_size; i++)
            {

                ListViewItem lvi;

                lvi = new ListViewItem();
                lvi.Text += Inst[i].name;
                if (Inst[i].op == 4) lvi.Text += "F" + Inst[i].rd + "," + Inst[i].imm + "(R" + Inst[i].rs + ")";
                else lvi.Text += "F" + Inst[i].rd + ",F" + Inst[i].rs + ".F" + Inst[i].rt;
                lvi.SubItems.Add(Inst[i].issueClock.ToString());
                lvi.SubItems.Add(Inst[i].executeClockBegin.ToString());
                lvi.SubItems.Add(Inst[i].executeClockEnd.ToString());
                lvi.SubItems.Add(Inst[i].writebackClock.ToString());
                listView1.Items.Add(lvi);
            }


        }


        private void Update_ReservationStation_Board()
        {
            listView2.Clear();
            listView2.View = View.Details;

            ColumnHeader clh;
            clh = new ColumnHeader();
            clh.Text = "   Name   ";
            listView2.Columns.Add(clh);
            clh = new ColumnHeader();
            clh.Text = "  Busy  ";
            listView2.Columns.Add(clh);
            clh = new ColumnHeader();
            clh.Text = "         OP        ";
            listView2.Columns.Add(clh);
            clh = new ColumnHeader();
            clh.Text = "          Vj              ";
            listView2.Columns.Add(clh);
            clh = new ColumnHeader();
            clh.Text = "              Vk             ";
            listView2.Columns.Add(clh);
            clh = new ColumnHeader();
            clh.Text = "     Qj      ";
            listView2.Columns.Add(clh);
            clh = new ColumnHeader();
            clh.Text = "     Qk      ";
            listView2.Columns.Add(clh);
            clh = new ColumnHeader();
            clh.Text = "     A       ";
            listView2.Columns.Add(clh);
            listView2.Columns[0].Width = -2;
            listView2.Columns[1].Width = -2;
            listView2.Columns[2].Width = -2;
            listView2.Columns[3].Width = -2;
            listView2.Columns[4].Width = -2;
            listView2.Columns[5].Width = -2;
            listView2.Columns[6].Width = -2;
            listView2.Columns[7].Width = -2;

            //下面添加信息

            for (int i = 0; i < ResStation_size; i++)
            {
                ListViewItem lvi;
                lvi = new ListViewItem();
                lvi.Text += ResStation[i].name;

                lvi.SubItems.Add(ResStation[i].busy.ToString());

                if (ResStation[i].busy) { 
                    switch (ResStation[i].op)
                    {
                        case 0:
                            lvi.SubItems.Add("ADD.D");
                            break;
                        case 1:
                            lvi.SubItems.Add("SUB.D");
                            break;
                        case 2:
                            lvi.SubItems.Add("MULT.D");
                            break;
                        case 3:
                            lvi.SubItems.Add("DIV.D");
                            break;
                        case 4:
                            lvi.SubItems.Add("L.D");
                            break;
                    }
                    if (ResStation[i].Qj == OperandAvailable)
                        lvi.SubItems.Add(ResStation[i].Vj.ToString());
                    else lvi.SubItems.Add("        ");
                    if (ResStation[i].Qk == OperandAvailable)
                        lvi.SubItems.Add(ResStation[i].Vk.ToString());
                    else lvi.SubItems.Add("        ");
                }
                else
                {
                    lvi.SubItems.Add("        ");
                    lvi.SubItems.Add("        ");
                    lvi.SubItems.Add("        ");
                }



                if(ResStation[i].Qj==OperandInit) lvi.SubItems.Add("        ");
                else if (ResStation[i].Qj==OperandAvailable) lvi.SubItems.Add("          ");
                else lvi.SubItems.Add(ResStation[ResStation[i].Qj].name);

                if (ResStation[i].Qk == OperandInit) lvi.SubItems.Add("        ");
                else if (ResStation[i].Qk == OperandAvailable) lvi.SubItems.Add("          ");
                else lvi.SubItems.Add(ResStation[ResStation[i].Qk].name);

                if(ResStation[i].op == LoadOp&&ResStation[i].busy == true) lvi.SubItems.Add(ResStation[i].A);
              
                listView2.Items.Add(lvi);




            }


        }



        private void Update_Register_Board()
        {
            listView3.Clear();
            listView3.View = View.Details;

            ColumnHeader clh;
            clh = new ColumnHeader();
            clh.Text = "        Field       ";
            listView3.Columns.Add(clh);
            clh = new ColumnHeader();
            clh.Text = "     F0     ";
            listView3.Columns.Add(clh);
            clh = new ColumnHeader();
            clh.Text = "     F1     ";
            listView3.Columns.Add(clh);
            clh = new ColumnHeader();
            clh.Text = "     F2     ";
            listView3.Columns.Add(clh);
            clh = new ColumnHeader();
            clh.Text = "     F3     ";
            listView3.Columns.Add(clh);
            clh = new ColumnHeader();
            clh.Text = "     F4     ";
            listView3.Columns.Add(clh);
            clh = new ColumnHeader();
            clh.Text = "     F5     ";
            listView3.Columns.Add(clh);
            clh = new ColumnHeader();
            clh.Text = "     F6     ";
            listView3.Columns.Add(clh);
            clh = new ColumnHeader();
            clh.Text = "     F7     ";
            listView3.Columns.Add(clh);
            clh = new ColumnHeader();
            clh.Text = "     F8     ";
            listView3.Columns.Add(clh);
            clh = new ColumnHeader();
            clh.Text = "     F9     ";
            listView3.Columns.Add(clh);
            clh = new ColumnHeader();
            clh.Text = "     F10    ";
            listView3.Columns.Add(clh);
            clh = new ColumnHeader();
            clh.Text = "     F11    ";
            listView3.Columns.Add(clh);
            clh = new ColumnHeader();
            clh.Text = "     F12    ";
            listView3.Columns.Add(clh);


            listView3.Columns[0].Width = -2;
            listView3.Columns[1].Width = -2;
            listView3.Columns[2].Width = -2;
            listView3.Columns[3].Width = -2;
            listView3.Columns[4].Width = -2;
            listView3.Columns[5].Width = -2;
            listView3.Columns[6].Width = -2;
            listView3.Columns[7].Width = -2;
            listView3.Columns[8].Width = -2;
            listView3.Columns[9].Width = -2;
            listView3.Columns[10].Width = -2;
            listView3.Columns[11].Width = -2;
            listView3.Columns[12].Width = -2;
            listView3.Columns[13].Width = -2;


            ListViewItem lvi;
            lvi = new ListViewItem();
            lvi.Text += "         Qi   ";
             
            for (int i = 0; i < RegisterStatus_size; i++)
            {
                if (RegisterStatus[i].Qi == RegStatusEmpty) lvi.SubItems.Add("     ");
                else lvi.SubItems.Add(ResStation[RegisterStatus[i].Qi].name);
            }
            listView3.Items.Add(lvi);

        }

        private void RunOneCycle()
        {
            ISSUE(Inst, ResStation, RegisterStatus);
            EXECUTE(Inst, ResStation, RegisterStatus);
            WRITEBACK(Inst, ResStation, RegisterStatus);
            Done = false;
            if (Total_WRITEBACKS == Inst_size)
                Done = true;
            Update_Instruction_Board();
            Update_ReservationStation_Board();
            Update_Register_Board();
        }


        private void Button1_Click(object sender, EventArgs e)
        {
            Clock++;
            RunOneCycle();
            Update_Clock_Board();
        }


    }
}
