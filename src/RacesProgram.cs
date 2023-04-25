using System.IO;
using System.Windows.Forms;

namespace RD_AAOW
	{
	/// <summary>
	/// ����� ��������� ����� ����� ����������
	/// </summary>
	public static class RacesProgram
		{
		// ������� ����������
		private static string[] Pths = {
			// ��� ������� �����������
			"Background\\Back.xnb",
			"Background\\StartBack.xnb",

			// ������
			"Font\\BigFont.xnb",
			"Font\\DefFont.xnb",
			"Font\\MidFont.xnb",

			// ��� �������
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

			// ��� tiles
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
		/// ����� ����� ����������
		/// </summary>
		/// <param name="args">��������� ��������� ������</param>
		public static void Main (string[] args)
			{
			// �������������
			Application.EnableVisualStyles ();
			Application.SetCompatibleTextRenderingDefault (false);

			// �������� XPUN
			if (!Localization.IsXPUNClassAcceptable)
				return;

			// �������� ������� ������������ �����
			if (!RDGenerics.IsAppInstanceUnique (true))
				return;

			// ����������� ������� � ������� �� �������� ��������
			if (!RDGenerics.AcceptEULA ())
				return;
			RDGenerics.ShowAbout (true);

			// ���������� �������� �� ������� ���� ����������� ������
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
