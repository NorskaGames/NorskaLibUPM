# NorskaLib Spreadsheets
Switch language: [RUS](https://github.com/NorskaGames/NorskaLibUPM/blob/master/Spreadsheets/README.ru.md) | **ENG**

Unity Editor tool, which helps design game-logic database (such as characters, weapons, items stats etc.) using Google Spreadsheets.

## Compatibility
- Unity Engine 2021.3+

## Installation
To install this package using Unity Package Manager simply insert this address:
```
https://github.com/NorskaGames/NorskaLibUPM.git?path=/Spreadsheets
```
## Setting up the Spreadsheet

Create your Spreadsheet and make sure you enable access via link:
![spreadsheet-setup](https://drive.google.com/uc?id=12Zo-_fQFYK8n9ljWMkfWtwbYhUUCP7ks)

_**Hint:** Design your database as any relational database (at least stick to 1st normal form: avoid lists inside a cell)._
![db-design-practices](https://drive.google.com/uc?id=1cGzRClYvEsvtzYkAlZp_nDVymvRPsjS1)

## Setting up the Container

Define your custom DataContainer and any amount of Data classes as shown below:
```
using NorskaLib.Spreadsheets;

[CreateAssetMenu(fileName = "SpreadsheetContainer", menuName = "PROJECT_NAME/SpreadsheetContainer")]
public class SpreadsheetContainer : SpreadsheetContainerBase
{
    [PageName("SomeSpreadsheetPage")]
    public List<ExampleData> ExampleData;
}

[System.Serializable]
public class ExampleData
{
    public string Id;

    public float SomeFloat;
    public int SomeInt;
    public string SomeString;
}
```
_**Important!** Make sure you spell variables names exactly as columns headers in the spreadsheet._

Now you can create SpreadsheetContainer.asset which u can use like this:
![container-inspector](https://drive.google.com/uc?id=16Rg4NIyj5c8-Qjq5phW0konDMRMKNN21)
