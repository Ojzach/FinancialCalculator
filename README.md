# Financial Calculator

Financial Calculator is a Windows desktop app built with WPF and the MVVM pattern. It helps you plan how every paycheck should be distributed across taxes, recurring budgets, and savings goals so that your accounts stay aligned with your financial plan.

## Features

- **Paycheck deposit calculator** – Enter your paycheck amount and withholding percentages to instantly see take-home pay, tax totals, and the monthly coverage provided by the deposit.
- **Budget allocation workspace** – Organize fixed and flexible budgets, adjust their recommended deposit percentages, and review how each category affects your available income.
- **Financial institution management** – Track the banks and accounts that hold your funds, including quick add/edit flows with inline validation.
- **Contextual editing panels** – Edit accounts or budgets without leaving the main views thanks to modal-style overlays powered by shared stores.
- **Extensible architecture** – Core domain logic lives in the `Models` layer, UI state is coordinated through `Stores`, and `ViewModels` expose strongly-typed bindings for XAML views.

## Project Structure

```text
FinancialCalculator/
├── Views/                 # XAML views for each screen (Deposit Calculator, Accounts, Navigation)
├── ViewModels/            # MVVM view models and helper classes for binding logic
├── Models/                # Domain models for budgets, accounts, and transactions
├── Stores/                # Application-wide state containers (navigation, budgets, institutions)
├── Commands/              # Reusable ICommand implementations for UI interactions
├── Converters/            # Value converters used in XAML bindings
└── ResourceDictionaries/  # Shared styles, templates, and data templates
```

## Getting Started

### Prerequisites

- Windows 10 or later (WPF is Windows-only)
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)

### Clone and build

```bash
git clone <repo-url>
cd FinancialCalculator
dotnet restore
dotnet build
```

### Run the application

Because this is a WPF application, run it from a Windows environment:

```bash
dotnet run
```

The main window opens with the navigation panel on the left. Use it to switch between the deposit calculator and the list of banks/accounts.

## Usage Tips

- Start on the **Deposit Calculator** view to input your paycheck amount and withholding percentages. The take-home pay automatically flows into the budget tree so you can see how much remains after each allocation.
- Use the **Banks** view to add your financial institutions. Selecting an institution opens an edit panel where you can adjust account details or delete entries.
- The overlay editor can be closed with the **Close** button when you want to return to the main workspace without applying changes.

## Roadmap

- Expand the navigation menu with expense tracking, spending budgets, and savings goal views.
- Persist data to disk so accounts and budgets survive across sessions.
- Add automated tests around the budget allocation logic.

## Contributing

1. Fork the repository and create a feature branch.
2. Ensure the project builds (`dotnet build`).
3. Submit a pull request with a clear description of your changes.

## License

The project has not been assigned a formal license yet. Please open an issue if you need clarification before using the code.
