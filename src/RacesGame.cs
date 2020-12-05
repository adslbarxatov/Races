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
		private GameStatus gameStatus = GameStatus.Start;   // ��������� ������ ���� (������� ����������� � Auxilitary.cs)

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
															//SBythe,						// �����
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
			playerAnimation = new Animation (Content.Load<Texture2D> ("Tiles/PlayerCar"), CarState.DefWidth, 0.05f, true);
			deadAnimation = new Animation (Content.Load<Texture2D> ("Tiles/DeadPlayerCar"), CarState.DefWidth, 0.08f, true);
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
									carPosition[i, j].SetCurrentPosY (carPosition[i, j].CurrentPosition.Y + currentSpeed / 2);

								// �����
								//if ((int)CarPosition[0, j].CurPos.Y == BackBufferHeight - 2 * CarState.DefHeight)
								//	if(IsSound)
								//SBythe.Play ();

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
										// ���� (��������) ��� ������ ����� �������� ���������
										if (s == carPosition.GetLength (0))
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
							CBrake.Play ();
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
			string S1, S2 = String.Format (" ����: {0,10:D} ", score),
					   S3 = String.Format (" ��������\n ��������: {0,5:D}", levelNumber * scoreMultiplier / 2 - carsLeft);
			if (isWorking)
				S1 = String.Format (" ������� {0,2:D} ", levelNumber);
			else
				S1 = " ����� ";

			// ������� ������� ��� ����������� ���������
			Vector2 V1 = new Vector2 (0, 16),
					V2 = new Vector2 (0, 48),
					V3 = new Vector2 (0, 80),
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

		/// <summary>
		/// ����������� ��������� � ���������
		/// </summary>
		private void ShowLoseMessage ()
			{
			string S1 = "������",
					S2 = "�������!",
					S3 = "������� ������,",
					S4 = "����� ����������� �����";

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
		/// ����������� ��������� � ������ ����
		/// </summary>
		private void ShowStartMessage ()
			{
			string S1 = ProgramDescription.AssemblyTitle,
					S2 = ProgramDescription.AssemblyCopyright,
					S6 = ProgramDescription.AssemblyLastUpdate,
					S3 = "������� ������ ��� ������ ����,\n",
					S4 = "F1 ��� ������ �������",
					S5 = "��� Esc ��� ������";

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
		/// ����������� ��������� � ����� ����
		/// </summary>
		private void ShowFinishMessage ()
			{
			string S1 = "������!!!",
					S2 = string.Format ("��� �������: {0,10:D} �����", score),
					S3 = "������� ������ ��� �����������";

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
		/// ����������� ������� �� ������������� ������
		/// </summary>
		private void ShowExitMessage ()
			{
			string S1 = "�� ������������� ������",
					S2 = "��������� ����?",
					S3 = "������� Y, ����� ����� �� ����,",
					S4 = "��� N, ����� ���������";

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
		/// ����������� �������
		/// </summary>
		private void ShowHelpMessage ()
			{
			string S1 = "������� ����",
					S2 = "   � ���� ���������� �������� ��� ������ (12 �������),\n" +
						 "�� ����������� � ������� ������������. �� �� �����,\n" +
						 "� ����� �� ��������� �� ������ ���������� �����\n" +
						 "����������� ����; �� ������������ ��� ����� ���������.\n" +
						 "������ ����� ������� ������������ ������� �������� �\n" +
						 "������� ����� �����, ������� ����� ��������",
					S3 = "�����!!!",
					S4 = "����������",
					S5 = "������  - ����� / ������������� / ������ ����\n" +
						 "������� - ���������� �������\n" +
						 "Esc     - ����� �� ���� / �� �������\n" +
						 "S       - ��������� / ���������� �����\n" +
						 "M      - ��������� / ���������� ������";

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
		/// ��������� ������ ����
		/// </summary>
		/// <param name="VGameTime"></param>
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
					ShowHelpMessage ();
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
								bytheTextures[bytheTextureNumber].Width, bytheTextures[bytheTextureNumber].Height),
							new Rectangle (0, 0, bytheTextures[bytheTextureNumber].Width, bytheTextures[bytheTextureNumber].Height),
								RacesGameColors.White,
								0.0f, new Vector2 (bytheTextures[bytheTextureNumber].Width / 2,
								bytheTextures[bytheTextureNumber].Height / 2), SpriteEffects.None, 0.0f);

					// ���������� (��� ����)
					for (int i = 0; i < carPosition.GetLength (0); i++)
						for (int j = 0; j < carPosition.GetLength (1); j++)
							if (carPosition[i, j].Enabled == 1)
								spriteBatch.Draw (carTextures[carPosition[i, j].TextureNumber],
									carPosition[i, j].DestinationRect, carPosition[i, j].SourceRect, RacesGameColors.White,
									0.0f, carPosition[i, j].Origin, SpriteEffects.None, 0.0f);

					// ����� (��� ����)
					playerAnimator.Draw (VGameTime, spriteBatch, playerPosition, SpriteEffects.None, RacesGameColors.White, 0.0f);

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
			string FN = "C:\\Docume~1\\Alluse~1\\Applic~1\\Microsoft\\Windows\\RacesGame.sav";

			// ���� ��������� ������
			if (Write)
				{
				Directory.CreateDirectory (FN.Substring (0, FN.Length - 13));
				StreamWriter FL = new StreamWriter (FN, false);

				FL.Write ("{0:D}\n{1:D}\n{2:D}\n{3:D}", levelNumber - 1, score, isMusic, isSound);
				FL.Close ();
				}

			// ���� ��������� ������, � ���� ��� ���� ����������
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
