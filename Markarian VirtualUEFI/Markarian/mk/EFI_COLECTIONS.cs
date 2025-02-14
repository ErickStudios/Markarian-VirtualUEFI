using System;
using System.Windows.Input;

// MKEFI.EFI_COLECTIONS
// this is the efi colections things for define flags , vars and other things used for EFI SERVICES

namespace MKEFI {

    public class EFI_COLECTIONS
    {
        // FLAGS
        public const int EFI_FLAG1 = 354;
        public const int EFI_FLAG2 = 546;
        public const int EFI_FLAG3 = 894;

        // efi vars
        public const string EFI_FILE = ".efi";
        // files
        public const string EFI_SERVICES_efi_nh = "// EFI.NH module\r\n\r\nfs1: EFI\\DEPENDENCES\r./EFIKEYS.NH\r\n\r\n#ifcondition= <?defined: EFILIB/EFI.NH| isequal |true>\r\n\r\nif\r\n#EFI_FLAG1= 354\r\n#EFI_FLAG2= 546\r\n#EFI_FLAG3= 894\r\n\r\n// declare types\r\n_DECLARATION -vars \"EFI_CONST\"\r\n_DECLARATION -vars \"EFI_INSTANCE\"\r\n\r\n#EFI_CONST= 3\r\n#EFI_INSTANCE= ?efi_instance_mk?\r\nendif\r\n\r\nif\r\n#EFILIB/EFI.NH= _EFILIB_\r\nendif";
        public const string EFI_SERVICES_efikeys_nh = "// EFIKEYS.NH module\r\n\r\n// this only define the special keys whitout \"=\" is 33 the letters key is 1 to 26\r\n\r\nfs1: EFI\\DEPENDENCES\r./EFIFLGS.NH\n\n\r\n#ifcondition= <?defined: EFILIB/EFIKEYS.NH| isequal |true>\r\n\r\nif\r\n#EFI_KEY_SPACE= 27\r\n#EFI_KEY_ENTER= 28\r\n#EFI_KEY_SHIFT= 29\r\n#EFI_KEY_CAPSLOCK= 30\r\n#EFI_KEY_CONTROL= 31\r\n#EFI_KEY_BACKSPACE= 32\r\nendif\r\n\r\nif\r\n#EFILIB/EFIKEYS.NH= _EFILIB_\r\nendif";
        public const string EFI_SERVICES_efiflags_nh = "// EFIFLAS.NH module\r\n\r\n// this define the flags into the efi\r\n\r\nfs1: EFI\\DEPENDENCES\r\n\r\n#ifcondition= <?defined: EFILIB/EFIFLAS.NH| isequal |true>\r\n\r\nif\r\n#EFI_FLAGNONE= 0\r\n#EFI_FLAG1= 354\r\n#EFI_FLAG2= 546\r\n#EFI_FLAG3= 894\r\n#EFI_FLAG4= 934\r\n#EFI_FLAG5= 1005\r\n#EFI_FLAG6= 1034\r\nendif\r\n\r\nif\r\n#EFILIB/EFIFLAS.NH= _EFILIB_\r\nendif";

        // colections
        public enum EFI_FLAGS
        {
            None = 0,
            FLAG1 = 354,
            FLAG2 = 546,
            FLAG3 = 894,
            FLAG4 = 934,
            FLAG5 = 1005,
            FLAG6 = 1034,
        }

        public static string EFI_GET_FLAG_NAME(EFI_FLAGS flags)
        {
            if (flags == EFI_FLAGS.None) return "sure";
            if (flags == EFI_FLAGS.FLAG1) return "error on efi partition";
            if (flags == EFI_FLAGS.FLAG2) return "error on load file";
            if (flags == EFI_FLAGS.FLAG3) return "error on load main efi shell file";
            if (flags == EFI_FLAGS.FLAG4) return "error on executable";
            if (flags == EFI_FLAGS.FLAG5) return "EFI LIB error";
            if (flags == EFI_FLAGS.FLAG6) return "internal code error";

            return "";
        }

        public static EFI_FLAGS EFI_GET_FLAG_CODE(string flags)
        {
            if (flags == "sure") return EFI_FLAGS.None;
            if (flags == "error on efi partition") return EFI_FLAGS.FLAG1;
            if (flags == "error on load file") return EFI_FLAGS.FLAG2;
            if (flags == "error on load main efi shell file") return EFI_FLAGS.FLAG3;
            if (flags == "error on executable") return EFI_FLAGS.FLAG4;
            if (flags == "EFI LIB error") return EFI_FLAGS.FLAG5;
            if (flags == "internal code error") return EFI_FLAGS.FLAG6;

            return EFI_FLAGS.None;
        }

        // input keys ninja asm
        public static Key EFI_GET_KEY_FROM_NUM(string KEY)
        {
            if (KEY == "1") { return Key.A; }
            if (KEY == "2") { return Key.B; }
            if (KEY == "3") { return Key.C; }
            if (KEY == "4") { return Key.D; }
            if (KEY == "5") { return Key.E; }
            if (KEY == "6") { return Key.F; }
            if (KEY == "7") { return Key.G; }
            if (KEY == "8") { return Key.H; }
            if (KEY == "9") { return Key.I; }
            if (KEY == "10") { return Key.J; }
            if (KEY == "11") { return Key.K; }
            if (KEY == "12") { return Key.L; }
            if (KEY == "13") { return Key.M; }
            if (KEY == "14") { return Key.N; }
            if (KEY == "15") { return Key.O; }
            if (KEY == "16") { return Key.P; }
            if (KEY == "17") { return Key.Q; }
            if (KEY == "18") { return Key.R; }
            if (KEY == "19") { return Key.S; }
            if (KEY == "20") { return Key.T; }
            if (KEY == "21") { return Key.U; }
            if (KEY == "22") { return Key.V; }
            if (KEY == "23") { return Key.W; }
            if (KEY == "24") { return Key.X; }
            if (KEY == "25") { return Key.Y; }
            if (KEY == "26") { return Key.Z; }
            if (KEY == "27") { return Key.Space; }
            if (KEY == "28") { return Key.Enter; }
            if (KEY == "29") { return Key.LeftShift; }
            if (KEY == "30") { return Key.CapsLock; }
            if (KEY == "31") { return Key.LeftCtrl; }
            if (KEY == "32") { return Key.Back; }
            if (KEY == "33") { return Key.Escape; }

            return Key.None;
        }
        public static string EFI_GET_KEY_CODE(Key KEY)
        {
            if (KEY == Key.A) { return "1"; }
            if (KEY == Key.B) { return "2"; }
            if (KEY == Key.C) { return "3"; }
            if (KEY == Key.D) { return "4"; }
            if (KEY == Key.E) { return "5"; }
            if (KEY == Key.F) { return "6"; }
            if (KEY == Key.G) { return "7"; }
            if (KEY == Key.H) { return "8"; }
            if (KEY == Key.I) { return "9"; }
            if (KEY == Key.J) { return "10"; }
            if (KEY == Key.K) { return "11"; }
            if (KEY == Key.L) { return "12"; }
            if (KEY == Key.M) { return "13"; }
            if (KEY == Key.N) { return "14"; }
            if (KEY == Key.O) { return "15"; }
            if (KEY == Key.P) { return "16"; }
            if (KEY == Key.Q) { return "17"; }
            if (KEY == Key.R) { return "18"; }
            if (KEY == Key.S) { return "19"; }
            if (KEY == Key.T) { return "20"; }
            if (KEY == Key.U) { return "21"; }
            if (KEY == Key.V) { return "22"; }
            if (KEY == Key.W) { return "23"; }
            if (KEY == Key.X) { return "24"; }
            if (KEY == Key.Y) { return "25"; }
            if (KEY == Key.Z) { return "26"; }
            if (KEY == Key.Space) { return "27"; }
            if (KEY == Key.Enter) { return "28"; }
            if (KEY == Key.LeftShift) { return "29"; }
            if (KEY == Key.CapsLock) { return "30"; }
            if (KEY == Key.LeftCtrl) { return "31"; }
            if (KEY == Key.Back) { return "32"; }
            if (KEY == Key.Escape) { return "33"; }

            return "None";
        }

        public static string EFI_KEY_TO_CHAR(string KEY)
        {
            if (KEY == "1") { return "a"; }
            if (KEY == "2") { return "b"; }
            if (KEY == "3") { return "c"; }
            if (KEY == "4") { return "d"; }
            if (KEY == "5") { return "e"; }
            if (KEY == "6") { return "f"; }
            if (KEY == "7") { return "g"; }
            if (KEY == "8") { return "h"; }
            if (KEY == "9") { return "i"; }
            if (KEY == "10") { return "j"; }
            if (KEY == "11") { return "k"; }
            if (KEY == "12") { return "l"; }
            if (KEY == "13") { return "m"; }
            if (KEY == "14") { return "n"; }
            if (KEY == "15") { return "o"; }
            if (KEY == "16") { return "p"; }
            if (KEY == "17") { return "q"; }
            if (KEY == "18") { return "r"; }
            if (KEY == "19") { return "s"; }
            if (KEY == "20") { return "t"; }
            if (KEY == "21") { return "u"; }
            if (KEY == "22") { return "v"; }
            if (KEY == "23") { return "w"; }
            if (KEY == "24") { return "x"; }
            if (KEY == "25") { return "y"; }
            if (KEY == "26") { return "z"; }
            if (KEY == "27") { return " "; }
            if (KEY == "28") { return "\n"; }
            if (KEY == "33") { return "[escape]"; }

            return "";
        }

    }
}