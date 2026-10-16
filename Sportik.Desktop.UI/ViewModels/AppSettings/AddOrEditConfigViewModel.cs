using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Sportik.Desktop.Core.Common.DataExchange;
using Sportik.Desktop.Core.Common.Export;
using Sportik.Desktop.Core.Common.Import;
using Sportik.Desktop.Core.Models.AppSettings;
using Sportik.Desktop.Core.Services.Interfaces;
using Sportik.Desktop.UI.Models;
using Sportik.Desktop.UI.ViewModels.Statistics;

namespace Sportik.Desktop.UI.ViewModels.AppSettings
{
    internal sealed class AddOrEditConfigViewModel : PopUpViewModel
    {
        private ObservableCollection<DataExchangeTypeOption> _typeOptions;

        public ObservableCollection<DataExchangeTypeOption> TypeOptions
        {
            get => _typeOptions;
            private set
            {
                if (SetField(ref _typeOptions, value))
                {
                    SetField(ref _selectedTypeOption, value[0], nameof(SelectedTypeOption));
                    Type = SelectedTypeOption.Type;
                }
            }
        }

        private DataExchangeTypeOption _selectedTypeOption;

        public DataExchangeTypeOption SelectedTypeOption
        {
            get => _selectedTypeOption;
            set
            {
                if (SetField(ref _selectedTypeOption, value))
                {
                    Type = SelectedTypeOption.Type;
                }
            }
        }

        private DataExchangeType _type;

        public DataExchangeType Type
        {
            get => _type;
            private set => SetField(ref _type, value);
        }

        private ObservableCollection<ImportExportScopeOption> _scopeOptions;

        public ObservableCollection<ImportExportScopeOption> ScopeOptions
        {
            get => _scopeOptions;
            private set
            {
                if (SetField(ref _scopeOptions, value))
                {
                    SetField(ref _selectedScopeOption, value[0], nameof(SelectedScopeOption));
                    Scope = SelectedScopeOption.Scope;
                }
            }
        }

        private ImportExportScopeOption _selectedScopeOption;

        public ImportExportScopeOption SelectedScopeOption
        {
            get => _selectedScopeOption;
            set
            {
                if (SetField(ref _selectedScopeOption, value))
                {
                    Scope = SelectedScopeOption.Scope;
                }
            }
        }

        private ImportExportScope _scope;

        public ImportExportScope Scope
        {
            get => _scope;
            private set => SetField(ref _scope, value);
        }

        private string _googleSheetUrlOrId;

        public string GoogleSheetUrlOrId
        {
            get => _googleSheetUrlOrId;
            set => SetField(ref _googleSheetUrlOrId, value);
        }

        private string _exercisesSheetName;

        public string ExercisesSheetName
        {
            get => _exercisesSheetName;
            set => SetField(ref _exercisesSheetName, value);
        }

        private string _setsSetsSheetName;

        public string SetsSheetName
        {
            get => _setsSetsSheetName;
            set => SetField(ref _setsSetsSheetName, value);
        }

        private bool _validateDuplicates = true;

        public bool ValidateDuplicates
        {
            get => _validateDuplicates;
            set => SetField(ref _validateDuplicates, value);
        }

        private string _title = "Add configuration";

        public string Title
        {
            get => _title;
            private set => SetField(ref _title, value);
        }

        private string _submitText = "Add";

        public string SubmitText
        {
            get => _submitText;
            private set => SetField(ref _submitText, value);
        }

        public ReactiveRelayCommand SubmitCommand { get; }

        private IAppSettingsService AppSettingsService => App.ServiceProvider.GetRequiredService<IAppSettingsService>();

        private IDataExchangeConfig _editedConfig;

        public AddOrEditConfigViewModel()
        {
            TypeOptions = new ObservableCollection<DataExchangeTypeOption>
            {
                new DataExchangeTypeOption("Import", DataExchangeType.Import),
                new DataExchangeTypeOption("Export", DataExchangeType.Export)
            };

            ScopeOptions = new ObservableCollection<ImportExportScopeOption>
            {
                new ImportExportScopeOption("Exercises and Sets", ImportExportScope.ExercisesAndSets),
                new ImportExportScopeOption("Exercises only", ImportExportScope.Exercises),
                new ImportExportScopeOption("Sets only", ImportExportScope.Sets)
            };

            SubmitCommand = new ReactiveRelayCommand(Submit);
        }

        public void OpenAdd()
        {
            _editedConfig = null;

            Title = "Add configuration";
            SubmitText = "Add";

            Open();
        }

        public void OpenEdit(IDataExchangeConfig config)
        {
            string sheetUrlOrId = null;
            string exercisesSheetName = null;
            string setsSheetName = null;
            bool validateDuplicates = true;

            if (config is GoogleSheetsImporterConfig importerConfig)
            {
                sheetUrlOrId = importerConfig.SheetUrlOrId;
                exercisesSheetName = importerConfig.ExercisesSheetName;
                setsSheetName = importerConfig.SetsSheetName;
                validateDuplicates = importerConfig.ValidateDuplicates;
            }
            else if (config is GoogleSheetsExporterConfig exporterConfig)
            {
                sheetUrlOrId = exporterConfig.SheetUrlOrId;
                exercisesSheetName = exporterConfig.ExercisesSheetName;
                setsSheetName = exporterConfig.SetsSheetName;
            }

            _editedConfig = config;

            SelectedTypeOption = TypeOptions.FirstOrDefault(option => option.Type == config.Type) ?? TypeOptions[0];
            SelectedScopeOption = ScopeOptions.FirstOrDefault(option => option.Scope == GetScope(exercisesSheetName, setsSheetName)) ?? ScopeOptions[0];
            GoogleSheetUrlOrId = sheetUrlOrId;
            ExercisesSheetName = exercisesSheetName;
            SetsSheetName = setsSheetName;
            ValidateDuplicates = validateDuplicates;

            Title = "Edit configuration";
            SubmitText = "Save";

            Open();
        }

        private static ImportExportScope GetScope(string exercisesSheetName, string setsSheetName)
        {
            if (!string.IsNullOrWhiteSpace(exercisesSheetName) && !string.IsNullOrWhiteSpace(setsSheetName))
            {
                return ImportExportScope.ExercisesAndSets;
            }

            if (!string.IsNullOrWhiteSpace(exercisesSheetName))
            {
                return ImportExportScope.Exercises;
            }

            if (!string.IsNullOrWhiteSpace(setsSheetName))
            {
                return ImportExportScope.Sets;
            }

            return ImportExportScope.ExercisesAndSets;
        }

        private void Submit()
        {
            DataExchangeType type = Type;
            ImportExportScope scope = Scope;
            string googleSheetUrlOrId = GoogleSheetUrlOrId;
            string exercisesSheetName = scope == ImportExportScope.Exercises || scope == ImportExportScope.ExercisesAndSets ? ExercisesSheetName : null;
            string setsSheetName = scope == ImportExportScope.Sets || scope == ImportExportScope.ExercisesAndSets ? SetsSheetName : null;

            IDataExchangeConfig config = type switch
            {
                DataExchangeType.Import => new GoogleSheetsImporterConfig(googleSheetUrlOrId, exercisesSheetName, setsSheetName, ValidateDuplicates),
                DataExchangeType.Export => new GoogleSheetsExporterConfig(googleSheetUrlOrId, exercisesSheetName, setsSheetName),
                _ => throw new ArgumentOutOfRangeException(nameof(Type), Type, "Invalid data exchange type.")
            };

            AutomaticDataExchangeSettings settings = AppSettingsService.GetSettings<AutomaticDataExchangeSettings>();

            bool succeeded = _editedConfig == null
                ? settings.TryAdd(config)
                : settings.TryEdit(_editedConfig, config);

            if (succeeded)
            {
                _editedConfig = null;
                Close();
            }
        }
    }
}