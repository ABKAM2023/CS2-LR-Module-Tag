# EN
**[C#] [LR] Module - Tag** is a module for the Levels Ranks plugin. This module assigns clan tags to players based on their rank.

# Installation
1. Install [C# Levels Ranks Core](https://github.com/ABKAM2023/CS2-LevelsRanks-Core/tree/v1.0)
2. Download [C#] [LR] Module - Tag
3. Unpack the archive and upload it to the game server
4. Configure settings_tags.json
5. Restart the server

# Configuration file (settings_tags.json)
Each ID in the config is an ID from settings_ranks.json.
```
{
  "LR_Tags": {
    "Tags": {
      "access": "1", // Allow players to disable Clan-Tag [ 0 - no, 1 - yes ]

      // The number of ranks here should correspond to the number of ranks in ( addons\counterstrikesharp\configs\plugins\LevelsRanks\settings_ranks.json )
      // All ranks here should be listed in ascending order, starting from 1 and ending with the number you have configured

      "1": {
        "tag": "[Rank 1]" // Clan-Tag that will be displayed for a player with the first rank
      },
      "2": {
        "tag": "[Rank 2]"
      },
      "3": {
        "tag": "[Rank 3]"
      },
      "4": {
        "tag": "[Rank 4]"
      },
      "5": {
        "tag": "[Rank 5]"
      },
      "6": {
        "tag": "[Rank 6]"
      },
      "7": {
        "tag": "[Rank 7]"
      },
      "8": {
        "tag": "[Rank 8]"
      },
      "9": {
        "tag": "[Rank 9]"
      },
      "10": {
        "tag": "[Rank 10]"
      },
      "11": {
        "tag": "[Rank 11]"
      },
      "12": {
        "tag": "[Rank 12]"
      },
      "13": {
        "tag": "[Rank 13]"
      },
      "14": {
        "tag": "[Rank 14]"
      },
      "15": {
        "tag": "[Rank 15]"
      },
      "16": {
        "tag": "[Rank 16]"
      },
      "17": {
        "tag": "[Rank 17]"
      },
      "18": {
        "tag": "[Rank 18]"
      }
    }
  }
}
```

# Plugin Translation Configuration File (en.json)
```
{
    "menu.toggle_tag": "Enable/Disable clan tag",
    "chat.tag_enabled": "{DarkRed}[LR] {White}Clan tag {Green}enabled{White}.",
    "chat.tag_disabled": "{DarkRed}[LR] {White}Clan tag {DarkRed}disabled{White}."
}
```

# RU
**[C#] [LR] Module - Tag** это модуль для плагина Levels Ranks. Данный модуль выставляет игрокам клан-тег в зависимости от их звания.

# Установка
1. Установите [C# Levels Ranks Core](https://github.com/ABKAM2023/CS2-LevelsRanks-Core/tree/v1.0)
2. Скачайте [C#] [LR] Module - Tag
3. Распакуйте архив и загрузите его на игровой сервер
4. Настройте settings_tags.json
5. Перезапустите сервер

# Конфигурационный файл (settings_tags.json)
Каждый ID в конфиге — это ID из settings_ranks.json.
```
{
  "LR_Tags": {
    "Tags": {
      "access": "1", // Разрешить ли игрокам отключать Клан-Тег [ 0 - нет, 1 - да ]

      // Количество рангов здесь должно соответствовать кол-ву рангов в ( addons\counterstrikesharp\configs\plugins\LevelsRanks\settings_ranks.json )
      // Все ранги тут следует располагать в порядке возрастания, начиная от 1 и заканчивая тем количеством, которое вы настроили

      "1": {
        "tag": "[Rank 1]" // Клан-Тег, который будет высвечиваться у игрока с самым первым званием
      },
      "2": {
        "tag": "[Rank 2]"
      },
      "3": {
        "tag": "[Rank 3]"
      },
      "4": {
        "tag": "[Rank 4]"
      },
      "5": {
        "tag": "[Rank 5]"
      },
      "6": {
        "tag": "[Rank 6]"
      },
      "7": {
        "tag": "[Rank 7]"
      },
      "8": {
        "tag": "[Rank 8]"
      },
      "9": {
        "tag": "[Rank 9]"
      },
      "10": {
        "tag": "[Rank 10]"
      },
      "11": {
        "tag": "[Rank 11]"
      },
      "12": {
        "tag": "[Rank 12]"
      },
      "13": {
        "tag": "[Rank 13]"
      },
      "14": {
        "tag": "[Rank 14]"
      },
      "15": {
        "tag": "[Rank 15]"
      },
      "16": {
        "tag": "[Rank 16]"
      },
      "17": {
        "tag": "[Rank 17]"
      },
      "18": {
        "tag": "[Rank 18]"
      }
    }
  }
}
```

# Конфигурационный файл с переводами плагина (ru.json)
```
{
    "menu.toggle_tag": "Включить/Выключить клан-тег",
    "chat.tag_enabled": "{DarkRed}[LR] {White}Клан-тег {Green}включен{White}.",
    "chat.tag_disabled": "{DarkRed}[LR] {White}Клан-тег {DarkRed}отключен{White}."
}

```
