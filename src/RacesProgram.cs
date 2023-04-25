using System.IO;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает точку входа приложения
	/// </summary>
	public static class RacesProgram
		{
		// Ресурсы приложения
		private static string[] Pths = {
			// Все фоновые изображения
			"Background\\Back.xnb",
			"Background\\StartBack.xnb",

			// Шрифты
			"Font\\BigFont.xnb",
			"Font\\DefFont.xnb",
			"Font\\MidFont.xnb",

			// Все мелодии
			"Sounds\\Ate1.xnb",
			"Sounds\\Bythe.xnb",
			"Sounds\\CBrake.xnb",
			"Sounds\\CEng.xnb",
			"Sounds\\Failed.xnb",
			"Sounds\\Music1.wma",
			"Sounds\\Music1.xnb",
			"Sounds\\Music2.wma",
			"Sounds\\Music2.xnb",
			"Sounds\\NewL.xnb",
			"Sounds\\SoundOnOff.xnb",
			"Sounds\\SStop.xnb",
			"Sounds\\SStart.xnb",

			// Все tiles
			"Tiles\\Bythe00.xnb",
			"Tiles\\Bythe01.xnb",
			"Tiles\\Bythe02.xnb",
			"Tiles\\Bythe03.xnb",
			"Tiles\\Bythe04.xnb",
			"Tiles\\CarA01.xnb",
			"Tiles\\CarA02.xnb",
			"Tiles\\CarA03.xnb",
			"Tiles\\CarA04.xnb",
			"Tiles\\CarA05.xnb",
			"Tiles\\CarA06.xnb",
			"Tiles\\CarA07.xnb",
			"Tiles\\CarB01.xnb",
			"Tiles\\CarB02.xnb",
			"Tiles\\CarB03.xnb",
			"Tiles\\CarB04.xnb",
			"Tiles\\CarB05.xnb",
			"Tiles\\CarC01.xnb",
			"Tiles\\CarC02.xnb",
			"Tiles\\CarC03.xnb",
			"Tiles\\CarC04.xnb",
			"Tiles\\DeadPlayerCar.xnb",
			"Tiles\\Eatable.xnb",
			"Tiles\\PlayerCar.xnb"
			};

		/// <summary>
		/// Точка входа приложения
		/// </summary>
		/// <param name="args">Агрументы командной строки</param>
		public static void Main (string[] args)
			{
			// Инициализация
			Application.EnableVisualStyles ();
			Application.SetCompatibleTextRenderingDefault (false);

			// Контроль XPUN
			if (!Localization.IsXPUNClassAcceptable)
				return;

			// Проверка запуска единственной копии
			if (!RDGenerics.IsAppInstanceUnique (true))
				return;

			// Отображение справки и запроса на принятие Политики
			if (!RDGenerics.AcceptEULA ())
				return;
			RDGenerics.ShowAbout (true);

			// Выполнение проверки на наличие всех необходимых файлов
			for (int i = 0; i < Pths.Length; i++)
				if (!File.Exists (".\\Content\\Races\\" + Pths[i]))
					{
					RDGenerics.LocalizedMessageBox (RDMessageTypes.Error, "MissingFile");
					return;
					}

			using (RacesGame game = new RacesGame ())
				game.Run ();
			}
		}
	}
