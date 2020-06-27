using Newtonsoft.Json;
using NexusForever.NPCSpawnTool.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

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

        string _Filter;
        public string Filter
        {
            get => _Filter;
            set
            {
                if (_Filter == value) return;
                if (value == null) _Filter = string.Empty;
                _Filter = value;

                if(_Filter.Length < 3)
                {
                    ViewableCreatures = new ObservableCollection<NPCSpawnModel>(CreatureList.Where(x => !string.IsNullOrEmpty(x.Description)).OrderBy(x => x.Description));
                } else
                {
                    ViewableCreatures = new ObservableCollection<NPCSpawnModel>(CreatureList.Where(x => x.Description.ToLower().Contains(Filter.ToLower()) && !string.IsNullOrEmpty(x.Description)).OrderBy(x => x.Description));
                }
                NotifyPropertyChanged(nameof(Filter));

            }
        }

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
                return $"!entity add {SelectedNPC.Creature} {SelectedNPC.Type} {SelectedNPC.displayInfo} {SelectedNPC.OutfitInfo} {SelectedNPC.faction1} {SelectedNPC.faction2} {SelectedNPC.activePropId}";
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
            Clipboard.SetText(NPCCode);
        }

        void LoadDataFile()
        {
            var json = File.ReadAllText("NPCTemplates.json");
            CreatureList = JsonConvert.DeserializeObject<List<NPCSpawnModel>>(json);
            foreach(var creature in CreatureList)
            {
                creature.Description = creature.Description.Trim();
            }

        }
    }
}
