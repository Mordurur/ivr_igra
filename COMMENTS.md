Так как Юнити позволяет относительно легко переключаться между платформами, я сделал две готовые сборки, которые находятся в папке Builds.


1. Сборка под Windows

2. Сборка под android




В игре в меню «Настройки» можно включить или выключить кнопки экранного управления.
С выключенными кнопками управления схема управления такая:

	ESC – назад
	Пробел — взаимодействие с окружающей средой
	Стрелки — влево / вправо, прыжок, приседание
	Z, X, C — различные атаки 


В папке Project находится Юнити проект.
Чтобы создать новую сборку, нужно:

1. Загрузить этот проект в Юнити

2. В Юнити выбрать File -> Build Settings

3. Выбрать Windows или android

4. Собрать и запустить

5. Чтобы собрать проект для android, потребуется скачать дополнительные части Юнити, которые доступны в Unity Hub. По какой-то странной причине у меня не скачивалась JDK, поэтому мне пришлось скачивать ее с сайта Oracle https://www.oracle.com/ru/java/technologies/javase/javase-jdk8-downloads.html

5.5 При создании сборки для android могут произойти и другие ошибки. Например, у меня она не работала когда проект лежал в папке с русским названием. Также, мне потребовалось отключить брандмауэр и пересоздать проект чтобы оно сработало.

Поэтому я сделал две готовые сборки под android, если вдруг возникнут какие-то неполадки.


Сохранение о внутреигровом прогрессе находятся в C:\Users\[Имя пользователя]\AppData\LocalLow\MrMrdrr\SelfTale\SaveData