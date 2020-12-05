using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.IO;

namespace RD_AAOW
	{
	/// <summary>
	/// Класс описывает игру Гонки
	/// </summary>
	public class RacesGame: Game
		{
		/////////////////////////////////////////////////////////////////////////////////
		// ПЕРЕМЕННЫЕ

		// Драйвера игры
		private GraphicsDeviceManager graphics;             // Графика
		private SpriteBatch spriteBatch;                    // Sprite-отрисовка
		private KeyboardState keyboardState;                // Состояние клавиатуры
		private SpriteFont defFont, midFont, bigFont;       // Шрифты
		private Random rnd = new Random ();                 // ГСЧ

		/// <summary>
		/// Ширина окна
		/// </summary>
		public const int BackBufferWidth = 620;

		/// <summary>
		/// Высота окна
		/// </summary>
		public const int BackBufferHeight = 640;

		/// <summary>
		/// Ширина дорожной полосы
		/// </summary>
		public const int RoadLineWidth = 50;

		/// <summary>
		/// Количество дорожных полос
		/// </summary>
		public const int LinesQuantity = 8;

		/// <summary>
		/// Граница дороги
		/// </summary>
		public const int RoadLeft = 210;

		// Основное состояние игры (начало|игра|конец)
		private GameStatus gameStatus = GameStatus.Start;   // Начальный статус игры (статусы перечислены в Auxilitary.cs)

		// Описатели уровня и окна сообщений
		private int levelNumber = 0,                        // Номер текущего уровня
					carsLeft = 0,                           // Количество пройденных машин
					currentSpeed = 3;                       // Скорость игрока
		private const int levelsQuantity = 30;              // Число уровней
		private Texture2D back,                             // Фон игры
						  startBack;                        // Фон на старте
		private Vector2 backOffset;                         // Смещение текстуры дороги

		// Текущая позиция игрока и его объекты анимации
		private Vector2 playerPosition;                     // Текущая позиция
		private Animation playerAnimation, deadAnimation;   // Изображения анимации (движется, dead)
		private AnimationPlayer playerAnimator;             // Объект-анимация

		// Машины, съедобные объекты и прочие параметры
		private Texture2D[] carTextures,                    // Автомобили
							bytheTextures;                  // Околодорожные объекты
		private Animation eatable;
		private AnimationPlayer eatableAnimator;
		private CarState[,] carPosition = new CarState[LinesQuantity, 3];
		private Vector2 bythePosition,                      // Позиция околодорожного объекта
						eatablePosition;                    // Позиция съедобного объекта
		private int bytheTextureNumber = 0, bytheShow = 0;

		// Звуковые эффекты и их параметры
		private SoundEffect SFailed,                        // Поражение
							SStart, SStop, SOnOff,          // Старт, пауза, звук off/on
							SAte,                           // Съедение
							CBrake, CEng,                   // Торможение / разгон
															//SBythe,						// Обгон
							NewLev;                         // Новый уровень
		private int soundDelay = 0;                         // Пауза между звуками CBrake, CEng и прочими
		private bool isSound = true, isMusic = true;        // Звук и музыка в игре on/off

		// Параметры Alive и Working
		private bool isAlive = false, isWorking = false;

		// Очки
		private int score = 0;                              // Выигрыш
		private const int scoreMultiplier = 10;             // Множитель для очков
		private const int penalty = 99;                     // Штраф за проигрыш

		// Флаги отображения сообщений
		private bool showLoseMsg = false,                   // Сообщение о прохождении уровня
					 showExitMsg = false;                   // Подтверждение выхода

		// Согласователи клавиатуры
		private int kbdDelay = 1,               // Пауза в Update-итерациях перед следующим опросом клавиатуры
					kbdDelayTimer;              // Таймер для delay
		private const int kbdDefDelay = 25;     // Базовый delay при нажатии клавиши

		/// <summary>
		/// Конструктор. Формирует рабочую область и окно приложения
		/// </summary>
		public RacesGame ()
			{
			// Создание "окна" заданного размера
			graphics = new GraphicsDeviceManager (this);
			graphics.PreferredBackBufferWidth = BackBufferWidth;
			graphics.PreferredBackBufferHeight = BackBufferHeight;

			// Задание content-директории игры
			Content.RootDirectory = "Content/Races";
			}

		/// <summary>
		/// ИНИЦИАЛИЗАЦИЯ
		/// Функция выполняется один раз за игру, при её запуске
		/// Здесь располагаются все инициализации и начальные значения
		/// </summary>
		protected override void Initialize ()
			{
			// НАСТРОЙКА АППАРАТА ПРОРИСОВКИ
			spriteBatch = new SpriteBatch (GraphicsDevice);

			// СОЗДАНИЕ ГРАФИЧЕСКИХ ОБЪЕКТОВ
			// Разные текстуры машин
			carTextures = new Texture2D[]   {
				Content.Load<Texture2D> ("Tiles/CarA01"),
				Content.Load<Texture2D> ("Tiles/CarA02"),
				Content.Load<Texture2D> ("Tiles/CarA03"),
				Content.Load<Texture2D> ("Tiles/CarA04"),
				Content.Load<Texture2D> ("Tiles/CarA05"),
				Content.Load<Texture2D> ("Tiles/CarA06"),
				Content.Load<Texture2D> ("Tiles/CarA07"),
				Content.Load<Texture2D> ("Tiles/CarB01"),
				Content.Load<Texture2D> ("Tiles/CarB02"),
				Content.Load<Texture2D> ("Tiles/CarB03"),
				Content.Load<Texture2D> ("Tiles/CarB04"),
				Content.Load<Texture2D> ("Tiles/CarB05"),
				Content.Load<Texture2D> ("Tiles/CarC01"),
				Content.Load<Texture2D> ("Tiles/CarC02"),
				Content.Load<Texture2D> ("Tiles/CarC03"),
				Content.Load<Texture2D> ("Tiles/CarC04")
											};

			// Разные текстуры околодорожных объектов
			bytheTextures = new Texture2D[] {
				Content.Load<Texture2D> ("Tiles/Bythe00"),
				Content.Load<Texture2D> ("Tiles/Bythe01"),
				Content.Load<Texture2D> ("Tiles/Bythe02"),
				Content.Load<Texture2D> ("Tiles/Bythe03"),
				Content.Load<Texture2D> ("Tiles/Bythe04")
											};

			// Текстура съедобного объекта
			eatable = new Animation (Content.Load<Texture2D> ("Tiles/Eatable"), 48, 0.1f, true);
			eatableAnimator.PlayAnimation (eatable);

			// Черепаха при столкновении и движении
			playerAnimation = new Animation (Content.Load<Texture2D> ("Tiles/PlayerCar"), CarState.DefWidth, 0.05f, true);
			deadAnimation = new Animation (Content.Load<Texture2D> ("Tiles/DeadPlayerCar"), CarState.DefWidth, 0.08f, true);
			playerAnimator.PlayAnimation (playerAnimation);

			// СОЗДАНИЕ ЗВУКОВЫХ ЭФФЕКТОВ
			SFailed = Content.Load<SoundEffect> ("Sounds/Failed");
			SOnOff = Content.Load<SoundEffect> ("Sounds/SoundOnOff");
			SStart = Content.Load<SoundEffect> ("Sounds/SStart");
			SStop = Content.Load<SoundEffect> ("Sounds/SStop");
			SAte = Content.Load<SoundEffect> ("Sounds/Ate1");
			CBrake = Content.Load<SoundEffect> ("Sounds/CBrake");
			CEng = Content.Load<SoundEffect> ("Sounds/CEng");
			//SBythe = Content.Load<SoundEffect> ("Sounds/Bythe");
			NewLev = Content.Load<SoundEffect> ("Sounds/NewL");

			// СОЗДАНИЕ ШРИФТОВ
			defFont = Content.Load<SpriteFont> ("Font/DefFont");
			midFont = Content.Load<SpriteFont> ("Font/MidFont");
			bigFont = Content.Load<SpriteFont> ("Font/BigFont");

			// ЗАГРУЗКА ДОПОЛНИТЕЛЬНЫХ ТЕКСТУР
			back = Content.Load<Texture2D> ("Background/Back");
			startBack = Content.Load<Texture2D> ("Background/StartBack");

			// ЧТЕНИЕ НАСТРОЕК И РЕЗУЛЬТАТОВ ИГРЫ
			GameSettings (false);

			// НАСТРОЙКА МУЗЫКИ
			MediaPlayer.IsRepeating = true;
			if (isMusic)
				MediaPlayer.Play (Content.Load<Song> ("Sounds/Music2"));

			// Инициализация
			base.Initialize ();
			}

		/////////////////////////////////////////////////////////////////////////////////
		// ДИНАМИЧЕСКИЕ СОБЫТИЯ ИГРЫ

		/// <summary>
		/// Метод обновляет состояние игры в реальном времени
		/// </summary>
		/// <param name="VGameTime">Время игры</param>
		protected override void Update (GameTime VGameTime)
			{
			// Опрос клавиатуры с предотвращением повторов
			kbdDelayTimer++;
			kbdDelayTimer %= kbdDelay;
			if (kbdDelayTimer == 0)
				{
				if (KeyboardProc ())
					{
					kbdDelay = kbdDefDelay;
					kbdDelayTimer = 0;
					}
				else
					{
					kbdDelay = 1;
					}
				}
			KeyboardMoveProc ();

			// В ЗАВИСИМОСТИ ОТ СОСТОЯНИЯ ИГРЫ
			switch (gameStatus)
				{
				//////////////////////////////////////////////////////////////////
				case GameStatus.Playing:
					if (isAlive)
						{

						// Движение всех объектов
						if (isWorking)
							{
							// Движение текстуры дороги
							if (backOffset.Y + currentSpeed >= 0.0f)
								backOffset.Y = -back.Height / 2;
							else
								backOffset.Y += currentSpeed;

							// Движение съедобного объекта
							if (eatablePosition.Y + currentSpeed >= BackBufferHeight + eatable.FrameHeight)
								{
								eatablePosition.X = rnd.Next (RoadLeft + eatable.FrameWidth / 2,
									RoadLeft + RoadLineWidth * LinesQuantity - eatable.FrameWidth / 2);
								eatablePosition.Y = -BackBufferHeight;
								}
							else
								eatablePosition.Y += currentSpeed;

							// Движение околодорожного объекта
							if (bythePosition.Y > BackBufferHeight + bytheTextures[bytheTextureNumber].Height)
								{
								bytheTextureNumber = rnd.Next (bytheTextures.Length);
								bythePosition.X = (3 * RoadLeft / 2 - bytheTextures[bytheTextureNumber].Width) / 2;
								bythePosition.Y = -bytheTextures[bytheTextureNumber].Height;
								bytheShow = rnd.Next (2);
								}
							else
								bythePosition.Y += currentSpeed;

							// Движение машин
							for (int j = 0; j < carPosition.GetLength (1); j++)
								{
								for (int i = 0; i < carPosition.GetLength (0); i++)
									// Смещение всех машин
									carPosition[i, j].SetCurrentPosY (carPosition[i, j].CurrentPosition.Y + currentSpeed / 2);

								// Обгон
								//if ((int)CarPosition[0, j].CurPos.Y == BackBufferHeight - 2 * CarState.DefHeight)
								//	if(IsSound)
								//SBythe.Play ();

								// Выход за границы уровня
								if (carPosition[0, j].CurrentPosition.Y > BackBufferHeight + CarState.DefHeight)
								// При выходе за границы уровня машина возвращается наверх
									{
									int s = 0;
									for (int i = 0; i < carPosition.GetLength (0); i++)
										{
										score += carPosition[i, j].Enabled;
										carPosition[i, j].Enabled = rnd.Next (2);
										s += carPosition[i, j].Enabled;
										carPosition[i, j].TextureNumber = rnd.Next (carTextures.Length);
										carPosition[i, j].SetCurrentPosY (-BackBufferHeight);
										}

									if (levelNumber <= 12)
										{
										// Если (внезапно) все восемь машин окажутся активными
										if (s == carPosition.GetLength (0))
											carPosition[rnd.Next (carPosition.GetLength (0)), j].Enabled = 0;
										}
									else if (levelNumber <= 17)
										{
										// Если будет больше четырёх машин
										if (s > (carPosition.GetLength (0) / 2))
											{
											for (int i = 0; i < carPosition.GetLength (0); i++)
												carPosition[i, j].Enabled = 0;

											carPosition[rnd.Next (carPosition.GetLength (0)), j].Enabled = 1;
											carPosition[rnd.Next (carPosition.GetLength (0)), j].Enabled = 1;
											carPosition[rnd.Next (carPosition.GetLength (0)), j].Enabled = 1;
											carPosition[rnd.Next (carPosition.GetLength (0)), j].Enabled = 1;
											}
										}
									else if (levelNumber <= 22)
										{
										if (s > (carPosition.GetLength (0) / 4))
											{
											for (int i = 0; i < carPosition.GetLength (0); i++)
												carPosition[i, j].Enabled = 0;

											carPosition[rnd.Next (carPosition.GetLength (0)), j].Enabled = 1;
											carPosition[rnd.Next (carPosition.GetLength (0)), j].Enabled = 1;
											}
										}
									else if (levelNumber <= 28)
										{
										if (s > (carPosition.GetLength (0) / 8))
											{
											for (int i = 0; i < carPosition.GetLength (0); i++)
												carPosition[i, j].Enabled = 0;

											carPosition[rnd.Next (carPosition.GetLength (0)), j].Enabled = 1;
											}
										}
									else
										{
										carPosition[carPosition.GetLength (0) - 1, j].Enabled = 0;
										}

									// Начисление очков
									carsLeft++;
									}
								}
							}

						// Следующий уровень
						if (carsLeft >= levelNumber * scoreMultiplier / 2)
							{
							if (isSound)
								NewLev.Play ();

							carsLeft = 0;
							levelNumber++;
							if (levelNumber > levelsQuantity)
								LoadNextLevel ();
							currentSpeed = levelNumber + 2;

							// Запись настроек и результатов игры (в зависимости от того, есть они или нет)
							GameSettings (true);
							}

						// Проверка столкновений с машинами
						if (IsCollapted ())
							{
							// Звук
							MediaPlayer.Stop ();
							if (isSound)
								SFailed.Play ();

							// Запись настроек и результатов игры (в зависимости от того, есть они или нет)
							GameSettings (true);

							// Переключение состояния игры
							isAlive = isWorking = false;
							levelNumber--;
							carsLeft = 0;               // Это было бы слишком просто...
							playerAnimator.PlayAnimation (deadAnimation);

							// Отображение сообщения
							showLoseMsg = true;

							// Пересчёт очков
							score -= penalty;           // Размер штрафа

							// Перезапуск уровня произойдёт по нажатию клавиши Space
							}

						// Проверка съедений
						if (IsAte ())
							{
							// Удаление съеденного объекта
							eatablePosition.X = rnd.Next (RoadLeft + eatable.FrameWidth / 2,
									RoadLeft + RoadLineWidth * LinesQuantity - eatable.FrameWidth / 2);
							eatablePosition.Y = -back.Height;

							// Пересчёт очков
							score += 5 * scoreMultiplier;

							// Звук
							SAte.Play ();
							}
						}

					break;
					//////////////////////////////////////////////////////////////////
				}

			// Обновление игры
			base.Update (VGameTime);
			}

		/// <summary>
		/// ОБРАБОТКА СОБЫТИЙ КЛАВИАТУРЫ
		/// Низкоскоростные события
		/// </summary>
		private bool KeyboardProc ()
			{
			// Запрос к клавиатуре
			keyboardState = Keyboard.GetState ();

			// В НЕЗАВИСИМОСТИ ОТ СОСТОЯНИЯ ИГРЫ
			// Настройки звука
			if (!showExitMsg)
				{
				if (keyboardState.IsKeyDown (Keys.S))       // Sound on/off
					{
					isSound = !isSound;
					SOnOff.Play ();

					// Была нажата клавиша
					return true;
					}

				if (keyboardState.IsKeyDown (Keys.M))
					{
					if (isMusic)                            // Music on/off
						{
						isMusic = false;
						MediaPlayer.Stop ();
						}
					else
						{
						isMusic = true;
						MediaPlayer.Play (Content.Load<Song> ("Sounds/Music1"));
						}
					SOnOff.Play ();

					return true;
					}
				}

			// В ЗАВИСИМОСТИ ОТ СОСТОЯНИЯ ИГРЫ
			switch (gameStatus)
				{
				//////////////////////////////////////////////////////////////////
				case GameStatus.Start:
					// Немедленный выход
					if (keyboardState.IsKeyDown (Keys.Escape))
						this.Exit ();

					// Справка
					if (keyboardState.IsKeyDown (Keys.F1))
						{
						gameStatus = GameStatus.Help;

						return true;
						}

					// Переход далее
					if (keyboardState.IsKeyDown (Keys.Space))
						{
						// Переключение параметров
						gameStatus = GameStatus.Playing;

						// Загрузка уровня
						LoadNextLevel ();

						return true;
						}

					break;

				//////////////////////////////////////////////////////////////////
				case GameStatus.Help:
					// Возврат
					if (keyboardState.IsKeyDown (Keys.Escape))
						{
						gameStatus = GameStatus.Start;

						return true;
						}

					break;

				//////////////////////////////////////////////////////////////////
				case GameStatus.Playing:

					// Нажатие паузы и продолжения
					if (!showExitMsg)           // Нельзя ничего делать, если появилось сообщение о выходе
						{
						if (isAlive && keyboardState.IsKeyDown (Keys.Space))    // Pause
							{
							if (isWorking)
								{
								isWorking = false;

								if (isSound)
									SStop.Play ();
								}
							else                                                // Continue
								{
								isWorking = true;

								if (isSound)
									SStart.Play ();
								}

							return true;
							}

						// Нажатие клавиши продолжения
						if (keyboardState.IsKeyDown (Keys.Space) && !isWorking && !isAlive)
							{
							LoadNextLevel ();

							return true;
							}

						// Проверка на выход
						if (keyboardState.IsKeyDown (Keys.Escape))
							{
							// Пауза
							isWorking = false;

							// Сообщение
							showExitMsg = true;

							// Звук
							if (isSound)
								SStop.Play ();

							return true;
							}
						}

					// Попытка выхода
					if (showExitMsg)
						{
						// Выход из игры (yes)
						if (keyboardState.IsKeyDown (Keys.Y))
							this.Exit ();

						// Продолжение (back)
						if (keyboardState.IsKeyDown (Keys.N))
							{
							showExitMsg = false;

							return true;
							}
						}

					break;

				//////////////////////////////////////////////////////////////////
				case GameStatus.Finish:
					if (keyboardState.IsKeyDown (Keys.Space))
						{
						// Переключение
						gameStatus = GameStatus.Start;

						return true;
						}

					break;

					//////////////////////////////////////////////////////////////////
				}

			// Не было ни одного нажатия
			return false;
			}

		/// <summary>
		/// ОБРАБОТКА СОБЫТИЙ КЛАВИАТУРЫ
		/// Высокоскоростные события
		/// </summary>
		private void KeyboardMoveProc ()
			{
			keyboardState = Keyboard.GetState ();

			// Нажатие клавиш управления
			if ((gameStatus == GameStatus.Playing) && !showExitMsg && isWorking)
				{
				// ВЛЕВО
				if (keyboardState.IsKeyDown (Keys.Left) && (playerPosition.X - scoreMultiplier >=
					RoadLeft + CarState.DefWidth / 2 - 5))
					{
					playerPosition.X -= scoreMultiplier / 2;
					}

				// ВПРАВО
				if (keyboardState.IsKeyDown (Keys.Right) && (playerPosition.X + scoreMultiplier <=
					BackBufferWidth - CarState.DefWidth / 2 - 5))
					{
					playerPosition.X += scoreMultiplier / 2;
					}

				// ВНИЗ (HINT)
				if (keyboardState.IsKeyUp (Keys.Down) && keyboardState.IsKeyUp (Keys.Up) && (soundDelay != -1))
					soundDelay = -1;

				if (keyboardState.IsKeyDown (Keys.Down))
					{
					backOffset.Y -= currentSpeed / 2;
					eatablePosition.Y -= currentSpeed / 2;
					bythePosition.Y -= currentSpeed / 2;

					for (int j = 0; j < carPosition.GetLength (1); j++)
						{
						for (int i = 0; i < carPosition.GetLength (0); i++)
							// Смещение всех машин
							carPosition[i, j].SetCurrentPosY (carPosition[i, j].CurrentPosition.Y - currentSpeed / 2);
						}

					if (isSound)
						{
						soundDelay++;
						soundDelay %= 100;
						if (soundDelay == 0)
							CBrake.Play ();
						}
					}

				// ВВЕРХ (HINT)
				if (keyboardState.IsKeyDown (Keys.Up))
					{
					backOffset.Y += currentSpeed;
					eatablePosition.Y += currentSpeed;
					bythePosition.Y += currentSpeed;

					for (int j = 0; j < carPosition.GetLength (1); j++)
						{
						for (int i = 0; i < carPosition.GetLength (0); i++)
							// Смещение всех машин
							carPosition[i, j].SetCurrentPosY (carPosition[i, j].CurrentPosition.Y + currentSpeed);
						}

					if (isSound)
						{
						soundDelay++;
						soundDelay %= 95;
						if (soundDelay == 0)
							CEng.Play ();
						}
					}
				}
			}

		/// <summary>
		/// Отображение информации игры (очки и уровень)
		/// </summary>
		private void DrawInfo ()
			{
			// Строки для отображения
			string S1, S2 = String.Format (" Очки: {0,10:D} ", score),
					   S3 = String.Format (" Осталось\n обогнать: {0,5:D}", levelNumber * scoreMultiplier / 2 - carsLeft);
			if (isWorking)
				S1 = String.Format (" УРОВЕНЬ {0,2:D} ", levelNumber);
			else
				S1 = " ПАУЗА ";

			// Векторы позиций для отображения элементов
			Vector2 V1 = new Vector2 (0, 16),
					V2 = new Vector2 (0, 48),
					V3 = new Vector2 (0, 80),
					V4 = new Vector2 (16, BackBufferHeight - 32),
					V5 = new Vector2 (48, BackBufferHeight - 32);

			DrawShadowedString (midFont, S1, V1, RacesGameColors.Orange);
			DrawShadowedString (midFont, S2, V2, RacesGameColors.Green);
			if (isAlive)
				DrawShadowedString (midFont, S3, V3, RacesGameColors.Green);

			// Если есть музыка или звук, выводить соответствующий знак
			if (isMusic)
				DrawShadowedString (defFont, "[\x266B]", V4, RacesGameColors.Yellow);
			else
				DrawShadowedString (defFont, "[\x266B]", V4, RacesGameColors.Black);

			if (isSound)
				DrawShadowedString (defFont, "[\x266A]", V5, RacesGameColors.Yellow);
			else
				DrawShadowedString (defFont, "[\x266A]", V5, RacesGameColors.Black);
			}

		/// <summary>
		/// Отображение сообщения о проигрыше
		/// </summary>
		private void ShowLoseMessage ()
			{
			string S1 = "МАШИНА",
					S2 = "РАЗБИТА!",
					S3 = "Нажмите Пробел,",
					S4 = "чтобы попробовать снова";

			Vector2 V1 = new Vector2 (16, (BackBufferHeight - 250) / 2),
					V2 = new Vector2 (16, (BackBufferHeight - 180) / 2),
					V3 = new Vector2 (16, (BackBufferHeight - 60) / 2),
					V4 = new Vector2 (16, (BackBufferHeight - 10) / 2);

			DrawShadowedString (midFont, S1, V1, RacesGameColors.Red);
			DrawShadowedString (midFont, S2, V2, RacesGameColors.Red);
			if (!showExitMsg)
				{
				DrawShadowedString (defFont, S3, V3, RacesGameColors.Red);
				DrawShadowedString (defFont, S4, V4, RacesGameColors.Red);
				}
			}

		/// <summary>
		/// Отображение сообщения о начале игры
		/// </summary>
		private void ShowStartMessage ()
			{
			string S1 = ProgramDescription.AssemblyTitle,
					S2 = ProgramDescription.AssemblyCopyright,
					S6 = ProgramDescription.AssemblyLastUpdate,
					S3 = "Нажмите Пробел для начала игры,\n",
					S4 = "F1 для вывода справки",
					S5 = "или Esc для выхода";

			Vector2 V1 = new Vector2 ((BackBufferWidth - bigFont.MeasureString (S1).X) / 2,
						120),
					V2 = new Vector2 (BackBufferWidth - defFont.MeasureString (S6).X - 20,
						BackBufferHeight - 70),
					V6 = new Vector2 (BackBufferWidth - defFont.MeasureString (S6).X - 20,
						BackBufferHeight - 40),
					V3 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S3).X) / 2,
						BackBufferHeight / 2),
					V4 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S4).X) / 2,
						BackBufferHeight / 2 + 30),
					V5 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S5).X) / 2,
						BackBufferHeight / 2 + 60);

			spriteBatch.Draw (startBack, Vector2.Zero, RacesGameColors.White);
			DrawShadowedString (bigFont, S1, V1, RacesGameColors.Green);
			DrawShadowedString (defFont, S2, V2, RacesGameColors.Yellow);
			DrawShadowedString (defFont, S6, V6, RacesGameColors.Yellow);
			DrawShadowedString (defFont, S3, V3, RacesGameColors.LBlue);
			DrawShadowedString (defFont, S4, V4, RacesGameColors.LBlue);
			DrawShadowedString (defFont, S5, V5, RacesGameColors.LBlue);
			}

		/// <summary>
		/// Отображение сообщения о конце игры
		/// </summary>
		private void ShowFinishMessage ()
			{
			string S1 = "ПОБЕДА!!!",
					S2 = string.Format ("Ваш выигрыш: {0,10:D} очков", score),
					S3 = "Нажмите Пробел для продолжения";

			Vector2 V1 = new Vector2 ((BackBufferWidth - bigFont.MeasureString (S1).X) / 2,
						(BackBufferHeight - 400) / 2),
					V2 = new Vector2 ((BackBufferWidth - midFont.MeasureString (S2).X) / 2,
						(BackBufferHeight - 50) / 2),
					V3 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S3).X) / 2,
						(BackBufferHeight + 100) / 2);

			spriteBatch.Draw (startBack, Vector2.Zero, RacesGameColors.White);
			DrawShadowedString (bigFont, S1, V1, RacesGameColors.Green);
			DrawShadowedString (midFont, S2, V2, RacesGameColors.LBlue);
			DrawShadowedString (defFont, S3, V3, RacesGameColors.Orange);
			}

		/// <summary>
		/// Отображение запроса на подтверждение выхода
		/// </summary>
		private void ShowExitMessage ()
			{
			string S1 = "Вы действительно хотите",
					S2 = "завершить игру?",
					S3 = "Нажмите Y, чтобы выйти из игры,",
					S4 = "или N, чтобы вернуться";

			Vector2 V1 = new Vector2 (16, (BackBufferHeight) / 2),
					V2 = new Vector2 (16, (BackBufferHeight + 70) / 2),
					V3 = new Vector2 (16, (BackBufferHeight + 190) / 2),
					V4 = new Vector2 (16, (BackBufferHeight + 240) / 2);

			DrawShadowedString (midFont, S1, V1, RacesGameColors.Orange);
			DrawShadowedString (midFont, S2, V2, RacesGameColors.Orange);
			DrawShadowedString (defFont, S3, V3, RacesGameColors.Orange);
			DrawShadowedString (defFont, S4, V4, RacesGameColors.Orange);
			}

		/// <summary>
		/// Отображение справки
		/// </summary>
		private void ShowHelpMessage ()
			{
			string S1 = "Правила игры",
					S2 = "   В игре необходимо проехать всю трассу (12 уровней),\n" +
						 "не сталкиваясь с другими автомобилями. За их обгон,\n" +
						 "а также за пойманные на дороге бриллианты будут\n" +
						 "начисляться очки; за столкновения они будут сниматься.\n" +
						 "Каждый новый уровень предполагает большую скорость и\n" +
						 "большее число машин, которые нужно обогнать",
					S3 = "Удачи!!!",
					S4 = "Управление",
					S5 = "Пробел  - пауза / возобновление / начало игры\n" +
						 "Стрелки - управление машиной\n" +
						 "Esc     - выход из игры / из справки\n" +
						 "S       - включение / выключение звука\n" +
						 "M      - включение / выключение музыки";

			Vector2 V1 = new Vector2 ((BackBufferWidth - midFont.MeasureString (S1).X) / 2,
						BackBufferHeight / 2 - 290),
					V2 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S2).X) / 2,
						BackBufferHeight / 2 - 240),
					V3 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S3).X) / 2,
						BackBufferHeight / 2 - 120),
					V4 = new Vector2 ((BackBufferWidth - midFont.MeasureString (S4).X) / 2,
						BackBufferHeight / 2 - 60),
					V5 = new Vector2 ((BackBufferWidth - defFont.MeasureString (S5).X) / 2,
						BackBufferHeight / 2 - 10);

			spriteBatch.Draw (startBack, Vector2.Zero, RacesGameColors.White);
			DrawShadowedString (midFont, S1, V1, RacesGameColors.Yellow);
			DrawShadowedString (defFont, S2, V2, RacesGameColors.Orange);
			DrawShadowedString (defFont, S3, V3, RacesGameColors.Orange);
			DrawShadowedString (midFont, S4, V4, RacesGameColors.Yellow);
			DrawShadowedString (defFont, S5, V5, RacesGameColors.Orange);
			}

		/// <summary>
		/// Отрисовка уровня игры
		/// </summary>
		/// <param name="VGameTime"></param>
		protected override void Draw (GameTime VGameTime)
			{
			// Создание чистого окна и запуск рисования
			graphics.GraphicsDevice.Clear (RacesGameColors.Black);
			spriteBatch.Begin ();

			// В ЗАВИСИМОСТИ ОТ СОСТОЯНИЯ ИГРЫ
			switch (gameStatus)
				{
				//////////////////////////////////////////////////////////////////
				case GameStatus.Start:
					// Отображает стартовый экран
					ShowStartMessage ();
					break;

				//////////////////////////////////////////////////////////////////
				case GameStatus.Help:
					ShowHelpMessage ();
					break;

				//////////////////////////////////////////////////////////////////
				case GameStatus.Playing:
					// ОТОБРАЖЕНИЕ УРОВНЯ
					spriteBatch.Draw (back, backOffset, RacesGameColors.White);

					// ОТОБРАЖЕНИЕ ИЗОБРАЖЕНИЙ
					// Съедобный объект
					eatableAnimator.Draw (VGameTime, spriteBatch, eatablePosition, SpriteEffects.None,
						RacesGameColors.White, 0.0f);

					// Околодорожный объект
					if (bytheShow == 1)
						spriteBatch.Draw (bytheTextures[bytheTextureNumber],
							new Rectangle ((int)bythePosition.X, (int)bythePosition.Y,
								bytheTextures[bytheTextureNumber].Width, bytheTextures[bytheTextureNumber].Height),
							new Rectangle (0, 0, bytheTextures[bytheTextureNumber].Width, bytheTextures[bytheTextureNumber].Height),
								RacesGameColors.White,
								0.0f, new Vector2 (bytheTextures[bytheTextureNumber].Width / 2,
								bytheTextures[bytheTextureNumber].Height / 2), SpriteEffects.None, 0.0f);

					// Автомобили (ещё выше)
					for (int i = 0; i < carPosition.GetLength (0); i++)
						for (int j = 0; j < carPosition.GetLength (1); j++)
							if (carPosition[i, j].Enabled == 1)
								spriteBatch.Draw (carTextures[carPosition[i, j].TextureNumber],
									carPosition[i, j].DestinationRect, carPosition[i, j].SourceRect, RacesGameColors.White,
									0.0f, carPosition[i, j].Origin, SpriteEffects.None, 0.0f);

					// Игрок (над ними)
					playerAnimator.Draw (VGameTime, spriteBatch, playerPosition, SpriteEffects.None, RacesGameColors.White, 0.0f);

					// ОТОБРАЖЕНИЕ ИНФОРМАЦИИ УРОВНЯ
					DrawInfo ();

					// Отображение сообщений (если они вызваны)
					if (showLoseMsg)
						ShowLoseMessage ();

					if (showExitMsg)
						ShowExitMessage ();

					break;

				//////////////////////////////////////////////////////////////////

				case GameStatus.Finish:
					ShowFinishMessage ();
					break;
				}

			// Завершение рисования
			spriteBatch.End ();

			// Перерисовка
			base.Draw (VGameTime);
			}

		/// <summary>
		/// Загрузка следующего уровня игры
		/// </summary>
		private void LoadNextLevel ()
			{
			// Возобновление игры
			isAlive = true;

			// Запуск фоновой мелодии
			MediaPlayer.Stop ();
			if (isMusic)
				MediaPlayer.Play (Content.Load<Song> ("Sounds/Music1"));

			// Поиск следующего имеющегося уровня
			while (true)
				{
				// Поиск С АВТОСМЕЩЕНИЕМ НА СЛЕДУЮЩИЙ УРОВЕНЬ
				levelNumber++;
				if (levelNumber < levelsQuantity + 1)
					break;

				// Перезапуск с нулевого уровня в конце игры
				levelNumber = -1;
				isAlive = isWorking = false;
				gameStatus = GameStatus.Finish;
				if (isMusic)
					MediaPlayer.Play (Content.Load<Song> ("Sounds/Music2"));
				}

			// ПЕРЕЗАГРУЗКА МАССИВОВ МАШИН И СЪЕДОБНЫХ ОБЪЕКТОВ
			// Очистка
			GenerateLevelObjects ();

			// Установка стартовых параметров
			playerAnimator.PlayAnimation (playerAnimation);
			playerPosition.X = RoadLeft + RoadLineWidth * LinesQuantity / 2;
			playerPosition.Y = BackBufferHeight - CarState.DefHeight;

			// Смена сообщения
			showLoseMsg = false;
			}

		/// <summary>
		/// Метод проверяет столкновение с препятствиями
		/// </summary>
		private bool IsCollapted ()
			{
			// Проверка на столкновение с машиной
			for (int i = 0; i < carPosition.GetLength (0); i++)
				for (int j = 0; j < carPosition.GetLength (1); j++)
					if ((carPosition[i, j].Enabled == 1) &&
						(Math.Abs (playerPosition.X - carPosition[i, j].CurrentPosition.X) < CarState.DefWidth - 6) &&
						(Math.Abs (playerPosition.Y - carPosition[i, j].CurrentPosition.Y) < CarState.DefHeight - 12))
						return true;

			// Не было столкновений
			return false;
			}

		/// <summary>
		/// Метод проверяет съедение объекта
		/// </summary>
		private bool IsAte ()
			{
			if ((Math.Abs (playerPosition.Y - eatablePosition.Y) <
				CarState.DefHeight / 2 + eatable.FrameHeight / 2 - 10) &&
				(Math.Abs (playerPosition.X - eatablePosition.X) <
				CarState.DefWidth / 2 + eatable.FrameWidth / 2 - 15))
				return true;

			// Не было съедения
			return false;
			}

		/// <summary>
		/// Метод выводит строку текста
		/// </summary>
		/// <param name="VFont">Шрифт текста</param>
		/// <param name="VString">Строка текста</param>
		/// <param name="VPosition">Позиция отрисовки</param>
		/// <param name="VColor">Цвет текста</param>
		private void DrawShadowedString (SpriteFont VFont, string VString, Vector2 VPosition, Color VColor)
			{
			spriteBatch.DrawString (VFont, VString, VPosition + new Vector2 (1, 1), RacesGameColors.Black);
			spriteBatch.DrawString (VFont, VString, VPosition + new Vector2 (1, -1), RacesGameColors.Black);
			spriteBatch.DrawString (VFont, VString, VPosition + new Vector2 (-1, 1), RacesGameColors.Black);
			spriteBatch.DrawString (VFont, VString, VPosition + new Vector2 (-1, -1), RacesGameColors.Black);
			spriteBatch.DrawString (VFont, VString, VPosition, VColor);
			}

		/// <summary>
		/// Метод выполняет чтение / запись настроек игры
		/// </summary>
		/// <param name="Write">Флаг режима записи настроек</param>
		private void GameSettings (bool Write)
			{
			string FN = "C:\\Docume~1\\Alluse~1\\Applic~1\\Microsoft\\Windows\\RacesGame.sav";

			// Если требуется запись
			if (Write)
				{
				Directory.CreateDirectory (FN.Substring (0, FN.Length - 13));
				StreamWriter FL = new StreamWriter (FN, false);

				FL.Write ("{0:D}\n{1:D}\n{2:D}\n{3:D}", levelNumber - 1, score, isMusic, isSound);
				FL.Close ();
				}

			// Если требуется чтение, и файл при этом существует
			else if (File.Exists (FN))
				{
				StreamReader FL = new StreamReader (FN);

				levelNumber = int.Parse (FL.ReadLine ());
				currentSpeed = levelNumber + 2;
				score = int.Parse (FL.ReadLine ());
				isMusic = bool.Parse (FL.ReadLine ());
				isSound = bool.Parse (FL.ReadLine ());

				FL.Close ();
				}
			}

		/// <summary>
		/// Метод генерирует новые объекты игры
		/// </summary>
		private void GenerateLevelObjects ()
			{
			// Машины
			for (int j = 0; j < carPosition.GetLength (1); j++)
				{
				int s = 0, b;
				// Для одной полосы скорость всех машин одинакова
				for (int i = 0; i < carPosition.GetLength (0); i++)
					{
					b = rnd.Next (2);
					s += b;
					carPosition[i, j] = new CarState (rnd.Next (carTextures.Length), i, j, b);
					}

				// Если (внезапно) все восемь машин окажутся активными
				if (s == carPosition.GetLength (0))
					carPosition[rnd.Next (carPosition.GetLength (0)), j].Enabled = 0;
				}

			// Первый съедобный объект
			eatablePosition.X = rnd.Next (RoadLeft + eatable.FrameWidth / 2,
				RoadLeft + RoadLineWidth * LinesQuantity - eatable.FrameWidth / 2);
			eatablePosition.Y = -BackBufferHeight;

			// Первый околодорожный объект
			bytheTextureNumber = rnd.Next (bytheTextures.Length);
			bythePosition.X = (3 * RoadLeft / 2 - bytheTextures[bytheTextureNumber].Width) / 2;
			bythePosition.Y = -bytheTextures[bytheTextureNumber].Height;
			bytheShow = rnd.Next (2);
			}
		}
	}
