using System;
using System.IO;
using System.Data;

namespace MKEFI
{
    // UL
    public class EFI_SERVICES
    {
        // EFI FILES EXECUTATION CODES
        public const int EFI_E_OK = 0; // IF IS OK
        public const int EFI_E_FILEMISSING = 1; // IF THE EFI CANNOT FOUND THE FILE
        public const int EFI_E_INVALID = 2; // IF THE FILE IS INVALID OBJET
        public const int EFI_E_UNEXCEPTED = 3; // IF THE FILE RETURNS A UNEXCEPT CODE

        // FILE UTILITYS
        public string EFI_FILE_TO_RUN;

        // GET CODE NAME OF ERROR
        public string EFI_E_CODE_NAME(int code)
        {
            switch (code)
            {
                case EFI_E_OK:
                    return "None (0)";
                case EFI_E_FILEMISSING:
                    return "Panda (1 - FILE_MISSING)";
                case EFI_E_INVALID:
                    return "Sugar (2 - INVALID_EFI)";
                case EFI_E_UNEXCEPTED:
                    return "Purple (3 - UNEXCEPT_CODE)";
            }
            return "Undefined"; 
        }

        // DECOMPÍLE THE FILE TO BE READY FOR EXECUTE THE CODE
        public string EFI_DECOMPILE(string text)
        {
            string result;
            result = text.Replace("◊◊◊♫♪ӒӒӦ", "-");
            result = result.Replace("◊◊◊♫♪ӒҴѴ", "_");
            result = result.Replace("◊◊◊♫♪ӒӒӱ", "\"");
            result = result.Replace("◊◊◊♫♪Ӓẏ", "/");
            result = result.Replace("◊◊◊♫♪ủỨ", "{");
            result = result.Replace("◊◊◊♫♪ủA", "}");
            return result;
        }

        // PREPARE THE FILE TO RUN IT
        public int EFI_PRERUNFILE(string fileefi)
        {
            if (File.Exists(fileefi))
            {
                if (EFI_DECOMPILE(File.ReadAllText(fileefi)) == File.ReadAllText(fileefi))
                {
                    EFI_FILE_TO_RUN = "NO EXEPTED";
                    return EFI_E_INVALID;
                }
            }
            else
            {
                EFI_FILE_TO_RUN = "NO EXEPTED";
                return EFI_E_FILEMISSING;
            }

            EFI_FILE_TO_RUN = fileefi;
            return EFI_E_OK;
        }

        public string EFI_RUNCODE()
        {
            return EFI_DECOMPILE(File.ReadAllText(EFI_FILE_TO_RUN));
        }
    }
}