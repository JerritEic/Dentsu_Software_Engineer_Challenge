# Dentsu Software Engineer Programming Challenge

This is a .NET 8 WPF application developed for an application to Dentsu.
Given a list of ad costs, fees, and total budget, it uses the goal seek
algorithm to find the maximum budget for a new ad such that total costs 
are within budget.

See `Prompt.txt` for full problem description.

## Running
Download the pre-built version from [releases](https://github.com/JerritEic/Dentsu_Software_Engineer_Challenge/releases/latest)
and run `Dentsu_Software_Engineer_Challenge.exe`, or build it yourself by running `dotnet build` in the `./Dentsu_Software_Engineer_Challenge/` folder with .NET SDK 8 installed.
The build result will be in `./Dentsu_Software_Engineer_Challenge/bin/Debug/net8.0-windows/`.

## Usage
The executable will open a UI with various form controls to specify budgets, fees, and costs.  

![UI](../Resources/1.png)

In the top left, a dropdown can be used to specifies a parameter preset, which can then
be applied by clicking the `Load Preset` button. 

In the leftmost column, there are four parameter controls. `TotalBudget` is the maximum budget that can be spent.
`Agency Fee Percent` is the percent of total ad cost billed by the agency, and `Third Party Fee Percent`
is the percent of total third party ad costs billed by third parties. `Agency Hour Costs` is a flat rate for agency hours.

The center two columns labelled `In House Ad Budgets` and `Third Party Ad Budgets` are lists of ad costs for ads that do
and do not use Third Party tools, respectively. New values can be added to these lists in the bottommost, empty rows.

In the right column, a `Starting Guess` for a new ad budget can be entered, as well as a checkbox indicated if the new ad
will incur Third Party Tool fees. When the `Calculate Budget for New Ad` button is clicked, the goal seek algorithm is run
with the provided parameters, and the resulting budget allocated to a new ad is shown in `New Ad Budget`, as well as the 
total cost of all ads and fees in `Total Spent`.

## Testing
NUnit is used for unit testing. Tests are contained in `./Dentsu_Software_Engineer_Challenge/Tests/UnityTests.cs`