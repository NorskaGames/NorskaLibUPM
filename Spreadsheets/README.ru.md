# NorskaLib Spreadsheets
Выбор языка: **RUS** | [ENG](https://github.com/NorskaGames/NorskaLibUPM/blob/master/Spreadsheets/README.md)

Инструмент для редактора Unity, призванный помочь в дизайне и импорте базы данных игры (настроек персонажей, оружия, предметов и т. п.) из таблиц Google.

# Руководство
## Установка
Чтобы установить данный ассет через Unity Package Manager используйте адрес:
```
https://github.com/NorskaGames/NorskaLibUPM.git?path=/Spreadsheets
```
## Настройка таблицы

Создайте таблицу и убедитесь, что она доступна по ссылке:
![spreadsheet-setup](https://drive.google.com/uc?id=12Zo-_fQFYK8n9ljWMkfWtwbYhUUCP7ks)

_**Подсказка:** Проектируйте базу данных как любую другую БД (придерживайтесь хотя бы 1-ой нормальной формы: избегайте вложенных списков)._
![db-design-practices](https://drive.google.com/uc?id=1cGzRClYvEsvtzYkAlZp_nDVymvRPsjS1)

## Настройка контейнера

Создайте класс DataContainer и произвольный набор Data-классов как в примере ниже:
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
_**Важно!** Убедитесь, что имена переменных совпадают с именами столбцов в таблице._

Теперь вы можете создать SpreadsheetContainer.asset и импортировать в него таблицу как показано на примере ниже:
![container-inspector](https://drive.google.com/uc?id=16Rg4NIyj5c8-Qjq5phW0konDMRMKNN21)
