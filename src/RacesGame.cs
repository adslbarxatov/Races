using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace RD_AAOW
	{
	/// <summary>
	/// ����� ��������� ���� �����
	/// </summary>
	public class RacesGame: Game
		{
		/////////////////////////////////////////////////////////////////////////////////
		// ����������

		// �������� ����
		private GraphicsDeviceManager graphics;             // �������
		private SpriteBatch spriteBatch;                    // Sprite-���������
		private KeyboardState keyboardState;                // ��������� ����������
		private SpriteFont defFont, midFont, bigFont;       // ������
		private Random rnd = new Random ();                 // ���

		/// <summary>
		/// ������ ����
		/// </summary>
		public const int BackBufferWidth = 620;

		/// <summary>
		/// ������ ����
		/// </summary>
		public const int BackBufferHeight = 640;

		/// <summary>
		/// ������ �������� ������
		/// </summary>
		public const int RoadLineWidth = 50;

		/// <summary>
		/// ���������� �������� �����
		/// </summary>
		public const int LinesQuantity = 8;

		/// <summary>
		/// ������� ������
		/// </summary>
		public const int RoadLeft = 210;

		// �������� ��������� ���� (������|����|�����)
		private GameStatus gameStatus = GameStatus.Start;
		// ��������� ������ ���� (������� ����������� � Auxilitary.cs)

		// ��������� ������ � ���� ���������
		private int levelNumber = 0,                        // ����� �������� ������
					carsLeft = 0,                           // ���������� ���������� �����
					currentSpeed = 3;                       // �������� ������
		private const int levelsQuantity = 30;              // ����� �������
		private Texture2D back,                             // ��� ����
						  startBack;                        // ��� �� ������
		private Vector2 backOffset;                         // �������� �������� ������

		// ������� ������� ������ � ��� ������� ��������
		private Vector2 playerPosition;                     // ������� �������
		private Animation playerAnimation, deadAnimation;   // ����������� �������� (��������, dead)
		private AnimationPlayer playerAnimator;             // ������-��������

		// ������, ��������� ������� � ������ ���������
		private Texture2D[] carTextures,                    // ����������
							bytheTextures;                  // ������������� �������
		private Animation eatable;
		private AnimationPlayer eatableAnimator;
		private CarState[,] carPosition = new CarState[LinesQuantity, 3];
		private Vector2 bythePosition,                      // ������� �������������� �������
						eatablePosition;                    // ������� ���������� �������
		private int bytheTextureNumber = 0, bytheShow = 0;

		// �������� ������� � �� ���������
		private SoundEffect SFailed,                        // ���������
							SStart, SStop, SOnOff,          // �����, �����, ���� off/on
							SAte,                           // ��������
							CBrake, CEng,                   // ���������� / ������
															//SBythe,							// �����
							NewLev;                         // ����� �������
		private int soundDelay = 0;                         // ����� ����� ������� CBrake, CEng � �������
		private bool isSound = true, isMusic = true;        // ���� � ������ � ���� on/off

		// ��������� Alive � Working
		private bool isAlive = false, isWorking = false;

		// ����
		private int score = 0;                              // �������
		private const int scoreMultiplier = 10;             // ��������� ��� �����
		private const int penalty = 99;                     // ����� �� ��������

		// ����� ����������� ���������
		private bool showLoseMsg = false,                   // ��������� � ����������� ������
					 showExitMsg = false;                   // ������������� ������

		// ������������� ����������
		private int kbdDelay = 1,               // ����� � Update-��������� ����� ��������� ������� ����������
					kbdDelayTimer;              // ������ ��� delay
		private const int kbdDefDelay = 25;     // ������� delay ��� ������� �������

		/// <summary>
		/// �����������. ��������� ������� ������� � ���� ����������
		/// </summary>
		public RacesGame ()
			{
			// �������� "����" ��������� �������
			graphics = new GraphicsDeviceManager (this);
			graphics.PreferredBackBufferWidth = BackBufferWidth;
			graphics.PreferredBackBufferHeight = BackBufferHeight;

			// ������� content-���������� ����
			Content.RootDirectory = "Content/Races";
			}

		/// <summary>
		/// �������������
		/// ������� ����������� ���� ��� �� ����, ��� � �������
		/// ����� ������������� ��� ������������� � ��������� ��������
		/// </summary>
		protected override void Initialize ()
			{
			// ��������� �������� ����������
			spriteBatch = new SpriteBatch (GraphicsDevice);

			// �������� ����������� ��������
			// ������ �������� �����
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

			// ������ �������� ������������� ��������
			bytheTextures = new Texture2D[] {
				Content.Load<Texture2D> ("Tiles/Bythe00"),
				Content.Load<Texture2D> ("Tiles/Bythe01"),
				Content.Load<Texture2D> ("Tiles/Bythe02"),
				Content.Load<Texture2D> ("Tiles/Bythe03"),
				Content.Load<Texture2D> ("Tiles/Bythe04")
				};

			// �������� ���������� �������
			eatable = new Animation (Content.Load<Texture2D> ("Tiles/Eatable"), 48, 0.1f, true);
			eatableAnimator.PlayAnimation (eatable);

			// �������� ��� ������������ � ��������
			playerAnimation = new Animation (Content.Load<Texture2D> ("Tiles/PlayerCar"),
				CarState.DefWidth, 0.05f, true);
			deadAnimation = new Animation (Content.Load<Texture2D> ("Tiles/DeadPlayerCar"),
				CarState.DefWidth, 0.08f, true);
			playerAnimator.PlayAnimation (playerAnimation);

			// �������� �������� ��������
			SFailed = Content.Load<SoundEffect> ("Sounds/Failed");
			SOnOff = Content.Load<SoundEffect> ("Sounds/SoundOnOff");
			SStart = Content.Load<SoundEffect> ("Sounds/SStart");
			SStop = Content.Load<SoundEffect> ("Sounds/SStop");
			SAte = Content.Load<SoundEffect> ("Sounds/Ate1");
			CBrake = Content.Load<SoundEffect> ("Sounds/CBrake");
			CEng = Content.Load<SoundEffect> ("Sounds/CEng");
			//SBythe = Content.Load<SoundEffect> ("Sounds/Bythe");
			NewLev = Content.Load<SoundEffect> ("Sounds/NewL");

			// �������� �������
			defFont = Content.Load<SpriteFont> ("Font/DefFont");
			midFont = Content.Load<SpriteFont> ("Font/MidFont");
			bigFont = Content.Load<SpriteFont> ("Font/BigFont");

			// �������� �������������� �������
			back = Content.Load<Texture2D> ("Background/Back");
			startBack = Content.Load<Texture2D> ("Background/StartBack");

			// ������ �������� � ����������� ����
			GameSettings (false);

			// ��������� ������
			MediaPlayer.IsRepeating = true;
			if (isMusic)
				MediaPlayer.Play (Content.Load<Song> ("Sounds/Music2"));

			// �������������
			base.Initialize ();
			}

		/////////////////////////////////////////////////////////////////////////////////
		// ������������ ������� ����

		/// <summary>
		/// ����� ��������� ��������� ���� � �������� �������
		/// </summary>
		/// <param name="VGameTime">����� ����</param>
		protected override void Update (GameTime VGameTime)
			{
			// ����� ���������� � ��������������� ��������
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

			// � ����������� �� ��������� ����
			switch (gameStatus)
				{
				//////////////////////////////////////////////////////////////////
				case GameStatus.Playing:
					if (isAlive)
						{

						// �������� ���� ��������
						if (isWorking)
							{
							// �������� �������� ������
							if (backOffset.Y + currentSpeed >= 0.0f)
								backOffset.Y = -back.Height / 2;
							else
								backOffset.Y += currentSpeed;

							// �������� ���������� �������
							if (eatablePosition.Y + currentSpeed >= BackBufferHeight + eatable.FrameHeight)
								{
								eatablePosition.X = rnd.Next (RoadLeft + eatable.FrameWidth / 2,
									RoadLeft + RoadLineWidth * LinesQuantity - eatable.FrameWidth / 2);
								eatablePosition.Y = -BackBufferHeight;
								}
							else
								eatablePosition.Y += currentSpeed;

							// �������� �������������� �������
							if (bythePosition.Y > BackBufferHeight + bytheTextures[bytheTextureNumber].Height)
								{
								bytheTextureNumber = rnd.Next (bytheTextures.Length);
								bythePosition.X = (3 * RoadLeft / 2 - bytheTextures[bytheTextureNumber].Width) / 2;
								bythePosition.Y = -bytheTextures[bytheTextureNumber].Height;
								bytheShow = rnd.Next (2);
								}
							else
								bythePosition.Y += currentSpeed;

							// �������� �����
							for (int j = 0; j < carPosition.GetLength (1); j++)
								{
								for (int i = 0; i < carPosition.GetLength (0); i++)
									// �������� ���� �����
									carPosition[i, j].SetCurrentPosY (carPosition[i, j].CurrentPosition.Y +
									currentSpeed / 2);

								// �����
#if false
								if ((int)carPosition[0, j].CurrentPosition.Y == BackBufferHeight - 
									2 * CarState.DefHeight)
									if (isSound)
										SBythe.Play ((60 + rnd.Next (20)) * 0.01f,
											(30 - rnd.Next (60)) * 0.01f, (50 - rnd.Next (100)) * 0.01f);
#endif

								// ����� �� ������� ������
								if (carPosition[0, j].CurrentPosition.Y > BackBufferHeight + CarState.DefHeight)
								// ��� ������ �� ������� ������ ������ ������������ ������
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
										// ���� (��������) ������ ��� ���� ����� �������� ���������
										if (s > carPosition.GetLength (0) - 1)
											carPosition[rnd.Next (carPosition.GetLength (0)), j].Enabled = 0;
										if (s > carPosition.GetLength (0) - 2)
											carPosition[rnd.Next (carPosition.GetLength (0)), j].Enabled = 0;
										}
									else if (levelNumber <= 17)
										{
										// ���� ����� ������ ������ �����
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

									// ���������� �����
									carsLeft++;
									}
								}
							}

						// ��������� �������
						if (carsLeft >= levelNumber * scoreMultiplier / 2)
							{
							if (isSound)
								NewLev.Play ();

							carsLeft = 0;
							levelNumber++;
							if (levelNumber > levelsQuantity)
								LoadNextLevel ();
							currentSpeed = levelNumber + 2;

							// ������ �������� � ����������� ���� (� ����������� �� ����, ���� ��� ��� ���)
							GameSettings (true);
							}

						// �������� ������������ � ��������
						if (IsCollapted ())
							{
							// ����
							MediaPlayer.Stop ();
							if (isSound)
								SFailed.Play ();

							// ������ �������� � ����������� ���� (� ����������� �� ����, ���� ��� ��� ���)
							GameSettings (true);

							// ������������ ��������� ����
							isAlive = isWorking = false;
							levelNumber--;
							carsLeft = 0;               // ��� ���� �� ������� ������...
							playerAnimator.PlayAnimation (deadAnimation);

							// ����������� ���������
							showLoseMsg = true;

							// �������� �����
							score -= penalty;           // ������ ������

							// ���������� ������ ��������� �� ������� ������� Space
							}

						// �������� ��������
						if (IsAte ())
							{
							// �������� ���������� �������
							eatablePosition.X = rnd.Next (RoadLeft + eatable.FrameWidth / 2,
									RoadLeft + RoadLineWidth * LinesQuantity - eatable.FrameWidth / 2);
							eatablePosition.Y = -back.Height;

							// �������� �����
							score += 5 * scoreMultiplier;

							// ����
							if (isSound)
								SAte.Play ();
							}
						}

					break;
					//////////////////////////////////////////////////////////////////
				}

			// ���������� ����
			base.Update (VGameTime);
			}

		/// <summary>
		/// ��������� ������� ����������
		/// ��������������� �������
		/// </summary>
		private bool KeyboardProc ()
			{
			// ������ � ����������
			keyboardState = Keyboard.GetState ();

			// � ������������� �� ��������� ����
			// ��������� �����
			if (!showExitMsg)
				{
				if (keyboardState.IsKeyDown (Keys.S))       // Sound on/off
					{
					isSound = !isSound;
					SOnOff.Play ();

					// ���� ������ �������
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

			// � ����������� �� ��������� ����
			switch (gameStatus)
				{
				//////////////////////////////////////////////////////////////////
				case GameStatus.Start:
					// ����������� �����
					if (keyboardState.IsKeyDown (Keys.Escape))
						this.Exit ();

					// �������
					if (keyboardState.IsKeyDown (Keys.F1))
						{
						gameStatus = GameStatus.Help;

						return true;
						}

					// ����� ����� ���������
					if (keyboardState.IsKeyDown (Keys.L))
						{
						gameStatus = GameStatus.Language;

						return true;
						}

					// ������� �����
					if (keyboardState.IsKeyDown (Keys.Space))
						{
						// ������������ ����������
						gameStatus = GameStatus.Playing;

						// �������� ������
						LoadNextLevel ();

						return true;
						}

					break;

				//////////////////////////////////////////////////////////////////
				case GameStatus.Help:
				case GameStatus.Language:
					// �������
					if (keyboardState.IsKeyDown (Keys.Escape))
						{
						gameStatus = GameStatus.Start;

						return true;
						}

					break;

				//////////////////////////////////////////////////////////////////
				case GameStatus.Playing:

					// ������� ����� � �����������
					if (!showExitMsg)           // ������ ������ ������, ���� ��������� ��������� � ������
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

						// ������� ������� �����������
						if (keyboardState.IsKeyDown (Keys.Space) && !isWorking && !isAlive)
							{
							LoadNextLevel ();

							return true;
							}

						// �������� �� �����
						if (keyboardState.IsKeyDown (Keys.Escape))
							{
							// �����
							isWorking = false;

							// ���������
							showExitMsg = true;

							// ����
							if (isSound)
								SStop.Play ();

							return true;
							}
						}

					// ������� ������
					if (showExitMsg)
						{
						// ����� �� ���� (yes)
						if (keyboardState.IsKeyDown (Keys.Y))
							this.Exit ();

						// ����������� (back)
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
						// ������������
						gameStatus = GameStatus.Start;

						return true;
						}

					break;

					//////////////////////////////////////////////////////////////////
				}

			// �� ���� �� ������ �������
			return false;
			}

		/// <summary>
		/// ��������� ������� ����������
		/// ���������������� �������
		/// </summary>
		private void KeyboardMoveProc ()
			{
			keyboardState = Keyboard.GetState ();

			// ������� ������ ����������
			if ((gameStatus == GameStatus.Playing) && !showExitMsg && isWorking)
				{
				// �����
				if (keyboardState.IsKeyDown (Keys.Left) && (playerPosition.X - scoreMultiplier >=
					RoadLeft + CarState.DefWidth / 2 - 5))
					{
					playerPosition.X -= scoreMultiplier / 2;
					}

				// ������
				if (keyboardState.IsKeyDown (Keys.Right) && (playerPosition.X + scoreMultiplier <=
					BackBufferWidth - CarState.DefWidth / 2 - 5))
					{
					playerPosition.X += scoreMultiplier / 2;
					}

				// ���� (HINT)
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
							// �������� ���� �����
							carPosition[i, j].SetCurrentPosY (carPosition[i, j].CurrentPosition.Y - currentSpeed / 2);
						}

					if (isSound)
						{
						soundDelay++;
						soundDelay %= 100;
						if (soundDelay == 0)
							CBrake.Play ((90 + rnd.Next (10)) * 0.01f,
								(10 - rnd.Next (20)) * 0.01f, 0.0f);
						}
					}

				// ����� (HINT)
				if (keyboardState.IsKeyDown (Keys.Up))
					{
					backOffset.Y += currentSpeed;
					eatablePosition.Y += currentSpeed;
					bythePosition.Y += currentSpeed;

					for (int j = 0; j < carPosition.GetLength (1); j++)
						{
						for (int i = 0; i < carPosition.GetLength (0); i++)
							// �������� ���� �����
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
		/// ����������� ���������� ���� (���� � �������)
		/// </summary>
		private void DrawInfo ()
			{
			// ������ ��� �����������
			if (string.IsNullOrWhiteSpace (stPoints))
				{
				stPoints = Localization.GetText ("Points");
				stCarsLeft = Localization.GetText ("CarsLeft");
				stLevel = Localization.GetText ("Level");
				stPause = Localization.GetText ("Pause");
				}

			string S1,
				S2 = string.Format (stPoints, score),
				S3 = string.Format (stCarsLeft, levelNumber * scoreMultiplier / 2 - carsLeft);
			if (isWorking)
				S1 = string.Format (stLevel, levelNumber);
			else
				S1 = stPause;

			// ������� ������� ��� ����������� ���������
			Vector2 V1 = new Vector2 (12, 16),
					V2 = new Vector2 (12, 48),
					V3 = new Vector2 (12, 80),
					V4 = new Vector2 (16, BackBufferHeight - 32),
					V5 = new Vector2 (48, BackBufferHeight - 32);

			DrawShadowedString (midFont, S1, V1, RacesGameColors.Orange);
			DrawShadowedString (midFont, S2, V2, RacesGameColors.Green);
			if (isAlive)
				DrawShadowedString (midFont, S3, V3, RacesGameColors.Green);

			// ���� ���� ������ ��� ����, �������� ��������������� ����
			if (isMusic)
				DrawShadowedString (defFont, "[\x266B]", V4, RacesGameColors.Yellow);
			else
				DrawShadowedString (defFont, "[\x266B]", V4, RacesGameColors.Black);

			if (isSound)
				DrawShadowedString (defFont, "[\x266A]", V5, RacesGameColors.Yellow);
			else
				DrawShadowedString (defFont, "[\x266A]", V5, RacesGameColors.Black);
			}
		private string stLevel, stPause, stCarsLeft, stPoints;

		/// <summary>
		/// ����������� ��������� � ���������
		/// </summary>
		private void ShowLoseMessage ()
			{
			// ������ �����
			if (string.IsNullOrWhiteSpace (stLoseLines[0]))
				{
				string[] values = Localization.GetText ("LoseLines").Split (splitter,
					StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < stLoseLines.Length; i++)
					stLoseLines[i] = values[i];
				}

			Vector2 V1 = new Vector2 (12, (BackBufferHeight - 250) / 2),
					V2 = new Vector2 (12, (BackBufferHeight - 180) / 2),
					V3 = new Vector2 (12, (BackBufferHeight - 60) / 2),
					V4 = new Vector2 (12, (BackBufferHeight - 10) / 2);

			DrawShadowedString (midFont, stLoseLines[0], V1, RacesGameColors.Red);
			DrawShadowedString (midFont, stLoseLines[1], V2, RacesGameColors.Red);
			if (!showExitMsg)
				{
				DrawShadowedString (defFont, stLoseLines[2], V3, RacesGameColors.Red);
				DrawShadowedString (defFont, stLoseLines[3], V4, RacesGameColors.Red);
				}
			}
		private string[] stLoseLines = new string[4];
		private char[] splitter = new char[] { '\t' };

		/// <summary>
		/// ����������� ��������� � ������ ����
		/// </summary>
		private void ShowStartMessage ()
			{
			// ������ �����
			if (string.IsNullOrWhiteSpace (stStartLines[0]))
				{
				string[] values = Localization.GetText ("StartLines").Split (splitter,
					StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < stStartLines.Length - 1; i++)
					stStartLines[i] = values[i];
				stStartLines[3] = ProgramDescription.AssemblyTitle;
				}

			Vector2 V1 = new Vector2 ((BackBufferWidth - bigFont.MeasureString (stStartLines[3]).X) / 2,
						120),
					V3 = new Vector2 ((BackBufferWidth - defFont.MeasureString (stStartLines[0]).X) / 2,
						BackBufferHeight / 2),
					V4 = new Vector2 ((BackBufferWidth - defFont.MeasureString (stStartLines[1]).X) / 2,
						BackBufferHeight / 2 + 30),
					V5 = new Vector2 ((BackBufferWidth - defFont.MeasureString (stStartLines[2]).X) / 2,
						BackBufferHeight / 2 + 60);

			spriteBatch.Draw (startBack, Vector2.Zero, RacesGameColors.White);
			DrawShadowedString (bigFont, stStartLines[3], V1, RacesGameColors.Green);
			DrawShadowedString (defFont, stStartLines[0], V3, RacesGameColors.LBlue);
			DrawShadowedString (defFont, stStartLines[1], V4, RacesGameColors.LBlue);
			DrawShadowedString (defFont, stStartLines[2], V5, RacesGameColors.LBlue);
			}
		private string[] stStartLines = new string[4];

		/// <summary>
		/// ����������� ��������� � ����� ����
		/// </summary>
		private void ShowFinishMessage ()
			{
			// ������ �����
			if (string.IsNullOrWhiteSpace (stSuccessLines[0]))
				{
				string[] values = Localization.GetText ("SuccessLines").Split (splitter,
					StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < stSuccessLines.Length; i++)
					stSuccessLines[i] = values[i];
				}
			string S01 = string.Format (stSuccessLines[1], score);

			Vector2 V1 = new Vector2 ((BackBufferWidth - bigFont.MeasureString (stSuccessLines[0]).X) / 2,
						(BackBufferHeight - 400) / 2),
					V2 = new Vector2 ((BackBufferWidth - midFont.MeasureString (S01).X) / 2,
						(BackBufferHeight - 50) / 2),
					V3 = new Vector2 ((BackBufferWidth - defFont.MeasureString (stSuccessLines[2]).X) / 2,
						(BackBufferHeight + 100) / 2);

			spriteBatch.Draw (startBack, Vector2.Zero, RacesGameColors.White);
			DrawShadowedString (bigFont, stSuccessLines[0], V1, RacesGameColors.Green);
			DrawShadowedString (midFont, S01, V2, RacesGameColors.LBlue);
			DrawShadowedString (defFont, stSuccessLines[2], V3, RacesGameColors.Orange);
			}
		private string[] stSuccessLines = new string[3];

		/// <summary>
		/// ����������� ������� �� ������������� ������
		/// </summary>
		private void ShowExitMessage ()
			{
			// ������ �����
			if (string.IsNullOrWhiteSpace (stExitLines[0]))
				{
				string[] values = Localization.GetText ("ExitLines").Split (splitter,
					StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < stExitLines.Length; i++)
					stExitLines[i] = values[i];
				}

			Vector2 V1 = new Vector2 (12, (BackBufferHeight) / 2),
					V2 = new Vector2 (12, (BackBufferHeight + 70) / 2),
					V3 = new Vector2 (12, (BackBufferHeight + 190) / 2),
					V4 = new Vector2 (12, (BackBufferHeight + 240) / 2);

			DrawShadowedString (midFont, stExitLines[0], V1, RacesGameColors.Orange);
			DrawShadowedString (midFont, stExitLines[1], V2, RacesGameColors.Orange);
			DrawShadowedString (defFont, stExitLines[2], V3, RacesGameColors.Orange);
			DrawShadowedString (defFont, stExitLines[3], V4, RacesGameColors.Orange);
			}
		private string[] stExitLines = new string[4];

		/// <summary>
		/// ����������� ��������������� �����������
		/// </summary>
		private void ShowServiceMessage (bool Language)
			{
			// ������ �� �������������� �����
			if (showingServiceMessage)
				return;
			showingServiceMessage = true;

			// ���������� ��������� � ������ �������
			spriteBatch.End ();

			if (Language)
				RDGenerics.MessageBox ();
			else
				RDGenerics.ShowAbout (false);

			spriteBatch.Begin ();

			// ����� � ����
			gameStatus = GameStatus.Start;
			showingServiceMessage = false;
			}
		private bool showingServiceMessage = false;

		/// <summary>
		/// ��������� ������ ����
		/// </summary>
		protected override void Draw (GameTime VGameTime)
			{
			// �������� ������� ���� � ������ ���������
			graphics.GraphicsDevice.Clear (RacesGameColors.Black);
			spriteBatch.Begin ();

			// � ����������� �� ��������� ����
			switch (gameStatus)
				{
				//////////////////////////////////////////////////////////////////
				case GameStatus.Start:
					// ���������� ��������� �����
					ShowStartMessage ();
					break;

				//////////////////////////////////////////////////////////////////
				case GameStatus.Help:
					ShowServiceMessage (false);
					break;

				case GameStatus.Language:
					ShowServiceMessage (true);
					break;

				//////////////////////////////////////////////////////////////////
				case GameStatus.Playing:
					// ����������� ������
					spriteBatch.Draw (back, backOffset, RacesGameColors.White);

					// ����������� �����������
					// ��������� ������
					eatableAnimator.Draw (VGameTime, spriteBatch, eatablePosition, SpriteEffects.None,
						RacesGameColors.White, 0.0f);

					// ������������� ������
					if (bytheShow == 1)
						spriteBatch.Draw (bytheTextures[bytheTextureNumber],
							new Rectangle ((int)bythePosition.X, (int)bythePosition.Y,
								bytheTextures[bytheTextureNumber].Width,
								bytheTextures[bytheTextureNumber].Height),
							new Rectangle (0, 0, bytheTextures[bytheTextureNumber].Width,
								bytheTextures[bytheTextureNumber].Height),
								RacesGameColors.White,
								0.0f, new Vector2 (bytheTextures[bytheTextureNumber].Width / 2,
								bytheTextures[bytheTextureNumber].Height / 2), SpriteEffects.None, 0.0f);

					// ���������� (��� ����)
					for (int i = 0; i < carPosition.GetLength (0); i++)
						for (int j = 0; j < carPosition.GetLength (1); j++)
							if (carPosition[i, j].Enabled == 1)
								spriteBatch.Draw (carTextures[carPosition[i, j].TextureNumber],
									carPosition[i, j].DestinationRect,
									carPosition[i, j].SourceRect, RacesGameColors.White,
									0.0f, carPosition[i, j].Origin, SpriteEffects.None, 0.0f);

					// ����� (��� ����)
					playerAnimator.Draw (VGameTime, spriteBatch, playerPosition, SpriteEffects.None,
						RacesGameColors.White, 0.0f);

					// ����������� ���������� ������
					DrawInfo ();

					// ����������� ��������� (���� ��� �������)
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

			// ���������� ���������
			spriteBatch.End ();

			// �����������
			base.Draw (VGameTime);
			}

		/// <summary>
		/// �������� ���������� ������ ����
		/// </summary>
		private void LoadNextLevel ()
			{
			// ������������� ����
			isAlive = true;

			// ������ ������� �������
			MediaPlayer.Stop ();
			if (isMusic)
				MediaPlayer.Play (Content.Load<Song> ("Sounds/Music1"));

			// ����� ���������� ���������� ������
			while (true)
				{
				// ����� � ������������� �� ��������� �������
				levelNumber++;
				if (levelNumber < levelsQuantity + 1)
					break;

				// ���������� � �������� ������ � ����� ����
				levelNumber = -1;
				isAlive = isWorking = false;
				gameStatus = GameStatus.Finish;
				if (isMusic)
					MediaPlayer.Play (Content.Load<Song> ("Sounds/Music2"));
				}

			// ������������ �������� ����� � ��������� ��������
			// �������
			GenerateLevelObjects ();

			// ��������� ��������� ����������
			playerAnimator.PlayAnimation (playerAnimation);
			playerPosition.X = RoadLeft + RoadLineWidth * LinesQuantity / 2;
			playerPosition.Y = BackBufferHeight - CarState.DefHeight;

			// ����� ���������
			showLoseMsg = false;
			}

		/// <summary>
		/// ����� ��������� ������������ � �������������
		/// </summary>
		private bool IsCollapted ()
			{
			// �������� �� ������������ � �������
			for (int i = 0; i < carPosition.GetLength (0); i++)
				for (int j = 0; j < carPosition.GetLength (1); j++)
					if ((carPosition[i, j].Enabled == 1) &&
						(Math.Abs (playerPosition.X - carPosition[i, j].CurrentPosition.X) < CarState.DefWidth - 6) &&
						(Math.Abs (playerPosition.Y - carPosition[i, j].CurrentPosition.Y) < CarState.DefHeight - 12))
						return true;

			// �� ���� ������������
			return false;
			}

		/// <summary>
		/// ����� ��������� �������� �������
		/// </summary>
		private bool IsAte ()
			{
			if ((Math.Abs (playerPosition.Y - eatablePosition.Y) <
				CarState.DefHeight / 2 + eatable.FrameHeight / 2 - 10) &&
				(Math.Abs (playerPosition.X - eatablePosition.X) <
				CarState.DefWidth / 2 + eatable.FrameWidth / 2 - 15))
				return true;

			// �� ���� ��������
			return false;
			}

		/// <summary>
		/// ����� ������� ������ ������
		/// </summary>
		/// <param name="VFont">����� ������</param>
		/// <param name="VString">������ ������</param>
		/// <param name="VPosition">������� ���������</param>
		/// <param name="VColor">���� ������</param>
		private void DrawShadowedString (SpriteFont VFont, string VString, Vector2 VPosition, Color VColor)
			{
			spriteBatch.DrawString (VFont, VString, VPosition + new Vector2 (1, 1), RacesGameColors.Black);
			spriteBatch.DrawString (VFont, VString, VPosition + new Vector2 (1, -1), RacesGameColors.Black);
			spriteBatch.DrawString (VFont, VString, VPosition + new Vector2 (-1, 1), RacesGameColors.Black);
			spriteBatch.DrawString (VFont, VString, VPosition + new Vector2 (-1, -1), RacesGameColors.Black);
			spriteBatch.DrawString (VFont, VString, VPosition, VColor);
			}

		/// <summary>
		/// ����� ��������� ������ / ������ �������� ����
		/// </summary>
		/// <param name="Write">���� ������ ������ ��������</param>
		private void GameSettings (bool Write)
			{
			// ���� ��������� ������
			if (Write)
				{
				RDGenerics.SetAppSettingsValue ("Level", (levelNumber - 1).ToString ());
				RDGenerics.SetAppSettingsValue ("Score", score.ToString ());
				RDGenerics.SetAppSettingsValue ("Music", isMusic.ToString ());
				RDGenerics.SetAppSettingsValue ("Sound", isSound.ToString ());
				}

			// ���� ��������� ������, � ���� ��� ���� ����������
			else
				{
				try
					{
					levelNumber = int.Parse (RDGenerics.GetAppSettingsValue ("Level"));
					currentSpeed = levelNumber + 2;
					score = int.Parse (RDGenerics.GetAppSettingsValue ("Score"));
					isMusic = bool.Parse (RDGenerics.GetAppSettingsValue ("Music"));
					isSound = bool.Parse (RDGenerics.GetAppSettingsValue ("Sound"));
					}
				catch { }
				}
			}

		/// <summary>
		/// ����� ���������� ����� ������� ����
		/// </summary>
		private void GenerateLevelObjects ()
			{
			// ������
			for (int j = 0; j < carPosition.GetLength (1); j++)
				{
				int s = 0, b;
				// ��� ����� ������ �������� ���� ����� ���������
				for (int i = 0; i < carPosition.GetLength (0); i++)
					{
					b = rnd.Next (2);
					s += b;
					carPosition[i, j] = new CarState (rnd.Next (carTextures.Length), i, j, b);
					}

				// ���� (��������) ��� ������ ����� �������� ���������
				if (s == carPosition.GetLength (0))
					carPosition[rnd.Next (carPosition.GetLength (0)), j].Enabled = 0;
				}

			// ������ ��������� ������
			eatablePosition.X = rnd.Next (RoadLeft + eatable.FrameWidth / 2,
				RoadLeft + RoadLineWidth * LinesQuantity - eatable.FrameWidth / 2);
			eatablePosition.Y = -BackBufferHeight;

			// ������ ������������� ������
			bytheTextureNumber = rnd.Next (bytheTextures.Length);
			bythePosition.X = (3 * RoadLeft / 2 - bytheTextures[bytheTextureNumber].Width) / 2;
			bythePosition.Y = -bytheTextures[bytheTextureNumber].Height;
			bytheShow = rnd.Next (2);
			}
		}
	}
