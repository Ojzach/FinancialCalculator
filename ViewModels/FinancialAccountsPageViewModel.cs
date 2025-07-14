using FinancialCalculator.Commands;
using FinancialCalculator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FinancialCalculator.ViewModels
{
    class FinancialAccountsPageViewModel : ViewModelBase
    {

        private ObservableCollection<FinancialInstitutionViewModel> financialInstitutions = new ObservableCollection<FinancialInstitutionViewModel>();
        public ObservableCollection<FinancialInstitutionViewModel> FinancialInstitutions { get => financialInstitutions; set { financialInstitutions = value; OnPropertyChanged(nameof(FinancialInstitutions)); } }

        private FinancialInstitutionViewModel ?selectedFinancialInstitution = null;
        public FinancialInstitutionViewModel? SelectedFinancialInstitution { get => selectedFinancialInstitution; set { selectedFinancialInstitution = value; OnPropertyChanged(nameof(SelectedFinancialInstitution)); } }

        private bool editAccountOpen = false;
        public Visibility EditAccountVisibility { get => editAccountOpen ? Visibility.Visible : Visibility.Collapsed; }


        public FinancialAccountsPageViewModel()
        {
            AddFinancialInstitution(new FinancialInstitution("USAA"));
            AddFinancialInstitution(new FinancialInstitution("Discover"));
            AddFinancialInstitution(new FinancialInstitution("Fidelity"));

            AddFinancialInstitutionCommand = new RelayCommand(execute => AddFinancialInstitution(new FinancialInstitution("")));
            DeleteFinancialInstitutionCommand = new RelayCommand(execute => DeleteFinancialInstitution(), canExecute => SelectedFinancialInstitution is not null);
            CloseEditMenuCommand = new RelayCommand(execute =>  CloseEditMenu());
        }



        public ICommand AddFinancialInstitutionCommand { get; set; }
        public void AddFinancialInstitution(FinancialInstitution institution)
        {
            FinancialInstitutions.Add(new FinancialInstitutionViewModel(institution));
            FinancialInstitutions[FinancialInstitutions.Count - 1].openEditAccount += OpenEditMenu;
        }

        public ICommand DeleteFinancialInstitutionCommand { get; set; }
        public void DeleteFinancialInstitution()
        {
            SelectedFinancialInstitution.openEditAccount -= OpenEditMenu;
            FinancialInstitutions.Remove(SelectedFinancialInstitution);
        }


        public ICommand CloseEditMenuCommand {  get; set; }

        public void OpenEditMenu(FinancialAccount account)
        {
            editAccountOpen = true;
            OnPropertyChanged(nameof(EditAccountVisibility));
        }

        public void CloseEditMenu()
        {
            editAccountOpen = false;
            OnPropertyChanged(nameof(EditAccountVisibility));
        }

    }
}
