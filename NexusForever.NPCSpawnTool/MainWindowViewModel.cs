using Newtonsoft.Json;
using NexusForever.NPCSpawnTool.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NexusForever.NPCSpawnTool
{
    public class MainWindowViewModel : ViewModelBase
    {

        [DllImport("user32.dll")]
        internal static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        internal static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        internal static extern bool SetClipboardData(uint uFormat, IntPtr data);

        public MainWindowViewModel()
        {
            LoadDataFile();
            ViewableCreatures = new ObservableCollection<NPCSpawnModel>(CreatureList.Where(x => !string.IsNullOrEmpty(x.Description)).OrderBy(x => x.Description));
        }

        List<NPCSpawnModel> CreatureList;

        NPCSpawnModel _SelectedNPC;
        public NPCSpawnModel SelectedNPC
        {
            get => _SelectedNPC;
            set
            {
                if (_SelectedNPC == value) return;
                _SelectedNPC = value;
                NotifyPropertyChanged(nameof(SelectedNPC));
            }
        }

        public string NPCCode
        {
            get
            {
                if (SelectedNPC == null) return string.Empty;
                return $"!entity add {SelectedNPC.Id}";
            }
        }
        ObservableCollection<NPCSpawnModel> _ViewableCreatures;
        public ObservableCollection<NPCSpawnModel> ViewableCreatures
        {
            get => _ViewableCreatures;
            set
            {
                if (_ViewableCreatures == value) return;
                _ViewableCreatures = value;
                NotifyPropertyChanged(nameof(ViewableCreatures));
            }
        }

        private RelayCommand _SpawnCodeCommand;
        public RelayCommand SpawnCodeCommand
        {
            get
            {
                return _SpawnCodeCommand
                  ?? (_SpawnCodeCommand = new RelayCommand(
                    article =>
                    {
                        CopySpawnCode();
                    }));
            }
        }

        void CopySpawnCode()
        {
            if (string.IsNullOrEmpty(NPCCode)) return;
            OpenClipboard(IntPtr.Zero);
            var ptr = Marshal.StringToHGlobalUni(NPCCode);
            SetClipboardData(13, ptr);
            CloseClipboard();
            Marshal.FreeHGlobal(ptr);
        }

        void LoadDataFile()
        {
            var json = File.ReadAllText("CreatureList.json");
            CreatureList = JsonConvert.DeserializeObject<List<NPCSpawnModel>>(json);

        }
    }
}
