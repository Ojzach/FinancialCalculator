using FinancialCalculator.Commands;
using FinancialCalculator.Models;
using FinancialCalculator.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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



        protected bool addFinancialInstitutionVisible = false;
        public Visibility AddFinancialInstitutionVisible { get => addFinancialInstitutionVisible ? Visibility.Visible : Visibility.Collapsed; }

        public string AddFinancialInstitutionName { get; set; } = "";

        private FinancialAccountViewModel currentlyEditingAccount;
        public FinancialAccountViewModel CurrentlyEditingAccount { get => currentlyEditingAccount; set { currentlyEditingAccount = value; OnPropertyChanged(nameof(CurrentlyEditingAccount)); } }


        private readonly FinancialInstitutionsStore _financialInstituitonsStore;

        public FinancialAccountsPageViewModel(FinancialInstitutionsStore financialInstitutionsStore)
        {
            _financialInstituitonsStore = financialInstitutionsStore;

            AddFinancialInstitution(new FinancialInstitution("USAA"));
            AddFinancialInstitution(new FinancialInstitution("Discover"));
            AddFinancialInstitution(new FinancialInstitution("Fidelity"));

            AddFinancialInstitutionCommand = new RelayCommand(
                execute => { AddFinancialInstitution(new FinancialInstitution(AddFinancialInstitutionName)); ToggleAddBox(); }, 
                canExecute => AddFinancialInstitutionName != "" && AddFinancialInstitutionName is not null);

            DeleteFinancialInstitutionCommand = new RelayCommand(
                execute => DeleteFinancialInstitution(), 
                canExecute => SelectedFinancialInstitution is not null);

            CloseEditMenuCommand = new RelayCommand(execute =>  CloseEditMenu());

            ToggleAddBoxCommand = new RelayCommand(execute => ToggleAddBox());
        }

        public void OpenPage()
        {
            foreach(FinancialInstitution fi in _financialInstituitonsStore.FinancialInstitutions)
            {
                FinancialInstitutions.Add(new FinancialInstitutionViewModel(fi));
            }
        }

        public void ClosePage()
        {
            _financialInstituitonsStore.UpdateFinancialInstitutionsList(FinancialInstitutions.ToList());


            FinancialInstitutions.Clear();
        }


        public RelayCommand ToggleAddBoxCommand { get; }

        private void ToggleAddBox()
        {
            addFinancialInstitutionVisible = !addFinancialInstitutionVisible;
            AddFinancialInstitutionName = "";
            OnPropertyChanged(nameof(AddFinancialInstitutionName));
            OnPropertyChanged(nameof(AddFinancialInstitutionVisible));
        }

        public ICommand AddFinancialInstitutionCommand { get; set; }
        public void AddFinancialInstitution(FinancialInstitution institution)
        {
            FinancialInstitutions.Add(new FinancialInstitutionViewModel(institution));
            FinancialInstitutions[FinancialInstitutions.Count - 1].openEditAccount += OpenEditMenu;
            FinancialInstitutions[FinancialInstitutions.Count - 1].openAddAccount += OpenAddMenu;
        }

        public ICommand DeleteFinancialInstitutionCommand { get; set; }
        public void DeleteFinancialInstitution()
        {
            SelectedFinancialInstitution.openEditAccount -= OpenEditMenu;
            FinancialInstitutions.Remove(SelectedFinancialInstitution);
        }


        public ICommand CloseEditMenuCommand {  get; set; }

        private bool editingItem = false;

        public void OpenEditMenu(FinancialAccountViewModel account)
        {
            editingItem = true;
            CurrentlyEditingAccount = account;
            editAccountOpen = true;
            OnPropertyChanged(nameof(EditAccountVisibility));
        }

        public void CloseEditMenu()
        {
            if(!editingItem) SelectedFinancialInstitution?.AddFinancialAccount(new FinancialAccount(CurrentlyEditingAccount));
            editAccountOpen = false;
            OnPropertyChanged(nameof(EditAccountVisibility));
            CurrentlyEditingAccount = null;
        }

        public void OpenAddMenu(FinancialInstitutionViewModel financialInstitution)
        {
            editingItem = false;
            SelectedFinancialInstitution = financialInstitution;
            CurrentlyEditingAccount = new FinancialAccountViewModel(new FinancialAccount("", BankAccountType.Checking));
            editAccountOpen = true;
            OnPropertyChanged(nameof(EditAccountVisibility));
        }

    }
}
