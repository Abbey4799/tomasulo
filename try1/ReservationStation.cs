using System.Collections.Generic;
using System.Linq;


namespace try1
{
    public class ReservationStation
    {
    public bool busy;
    public int Qj;
    public int Qk;
    public string Vj;
    public string Vk;
    public string A;
    public int lat;
    public int op;
    public int result;
    public bool resultReady;
    public int instNum;
    public int ISSUE_Lat;
    public int WRITEBACK_Lat;
    public string name;

    public ReservationStation(){
        busy = false;
        op = 0;
        lat = 0;
        result = 0;
        resultReady = false;
        Qj = 1;
        Qk = 1;
        instNum = 100000;
        ISSUE_Lat = 0;
        WRITEBACK_Lat = 0;

    }

    public ReservationStation(int OP, int RSoperandAvailable,string Name){
        busy = false;
        op = OP;
        lat = 0;
        result = 0;
        resultReady = false;
        Qj = RSoperandAvailable;
        Qk = RSoperandAvailable;
        instNum = 100000;
        ISSUE_Lat = 0;
        WRITEBACK_Lat = 0;
        name = Name;
    }
    }
}
