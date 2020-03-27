using System.Collections.Generic;
using System.Linq;


namespace try1
{
    public class RegisterStatus
    {
        public bool busy;
        public int Qi;

        public RegisterStatus(){
            busy = false;
            Qi = 0;
        }
        public RegisterStatus(int RegStatusEmpty) {
            Qi = RegStatusEmpty;
        }

    }
}
