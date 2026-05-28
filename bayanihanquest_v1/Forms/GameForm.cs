using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Windows_form_game_V1._0.Models;
using Windows_form_game_V1._0.Services;
namespace Windows_form_game_V1._0.Forms
{
    // Main game form - dito nangyayari yung buong gameplay
    public partial class GameForm : Form
    {
        private readonly Random random = new Random();
        private readonly List<Control> wallBlocks = new List<Control>();
        private readonly List<TrashItem> trashItems = new List<TrashItem>();
        private readonly List<IngredientItem> ingredientItems = new List<IngredientItem>();
        private readonly List<Rectangle> trashSpawnAreas = new List<Rectangle>();

        private Player player;
        private Quest activeQuest;
        private Quest map2Quest;

        private PictureBox playerSprite;
        private PictureBox captainNpc;
        private Label floatingHintLabel;

        private bool moveUp;
        private bool moveDown;
        private bool moveLeft;
        private bool moveRight;
        private bool nearCaptain;
        private bool nearStore;

        private Rectangle storeInteractionZone;
        private bool isStoreModalOpen;
        private bool currentMapAllowsTrash;
        private string currentMapKey = "map1";
        private bool hasLoadedProgress;
        private Point loadedPlayerLocation;
        private bool interactionLocked;
        private int interactionCooldownFrames;
        private const int InteractionCooldownFrames = 8;

        private Rectangle mapDownTransitionZone;
        private Rectangle mapUpTransitionZone;

        private double playerX;
        private double playerY;
        private int moveFrameCounter;

        private Image mapBackgroundImage;
        private Image map2BackgroundImage;
        private Image trashImage;
        private Image chickenImage;
        private Image coconutImage;
        private Image playerUpImage;
        private Image playerDownImage;
        private Image playerLeftImage;
        private Image playerRightImage;
        private Image captainImage;
        private string playerFacing = "down";
        private const int TrashCollectRadius = 44;
        private const int TrashCollectRadiusSquared = TrashCollectRadius * TrashCollectRadius;
        private const int IngredientCollectRadius = 42;
        private const int IngredientCollectRadiusSquared = IngredientCollectRadius * IngredientCollectRadius;
        private const int PantryChickenGoal = 5;
        private const int PantryCoconutGoal = 5;
        private const int BarangayFundGoal = 2000;
        private const int FramesPerInGameDay = 3800;
        private int chickenCollected;
        private int coconutCollected;
        private bool pantryMissionUnlocked;
        private bool pantryMissionInProgress;
        private bool pantryMissionCompleted;
        private int inGameDay = 1;
        private int dayFrameCounter;
        private int barangayFund;
        private int dailyMissionStreak;
        private int dailyTrashTarget = 8;
        private int dailyTrashProgress;
        private bool dailyMissionCompleted;
        private int streetlightLevel;
        private int wasteBinLevel;
        private int gardenLevel;
        private bool fundGoalAnnounced;
        private int lastNpcRequestDay;
        private int miniGamesCompleted;
        private bool communityEventActive;
        private string communityEventName = string.Empty;
        private int communityEventGoal;
        private int communityEventProgress;
        private int communityEventEndDay;
        private int communityEventsCompleted;

        public GameForm()
            : this(false)
        {
        }

        public GameForm(bool loadSavedProgress)
        {
            InitializeComponent();
            hasLoadedProgress = false;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            UpdateStyles();
            EnableDoubleBuffer(panelMap);
            DoubleBuffered = true;
            SetupGame(loadSavedProgress);
            StyleButtons();
            gameTimer.Start();
        }

        private static void EnableDoubleBuffer(Control control)
        {
            var doubleBufferProperty = typeof(Control).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            if (doubleBufferProperty != null)
            {
                doubleBufferProperty.SetValue(control, true, null);
            }
        }

        // Setup ng game - naglo-load ng assets, gumagawa ng player at quests
        private void SetupGame(bool loadSavedProgress)
        {
            LoadAssets();

            player = new Player();
            activeQuest = new Quest(
                "Clean Sweep",
                "Pick up 10 trash around the barangay and report back to the Barangay Captain.",
                10,
                250,
                15);

            map2Quest = new Quest(
                "Road Cleanup",
                "Collect 8 trash items along the road in Map 2.",
                8,
                180,
                10);

            if (loadSavedProgress)
            {
                TryLoadProgress();
            }

            CreateMapLayout();

            var currentQuest = GetCurrentMapQuest();
            if (currentQuest.Status == QuestStatus.InProgress)
            {
                SpawnTrash(GetTrashSpawnTarget());
            }

            if (currentMapKey == "map1" && pantryMissionInProgress && !pantryMissionCompleted)
            {
                SpawnIngredients(8);
            }

            UpdateHud();
        }

        // Important: dito kinukuha lahat ng sprite images at backgrounds
        private void LoadAssets()
        {
            mapBackgroundImage = Services.AssetLoader.LoadImage(
                "background for barangay.png",
                "background for baranggay.png",
                "barangay.png",
                "batangay.png");

            map2BackgroundImage = Services.AssetLoader.LoadImage("map2.png", "Map2.png");

            trashImage = Services.AssetLoader.LoadImage("banana-trash.png");
            chickenImage = MakeSpriteBackgroundTransparent(Services.AssetLoader.LoadImage("chicken.png")) ?? CreateIngredientFallbackImage("Chicken");
            coconutImage = MakeSpriteBackgroundTransparent(Services.AssetLoader.LoadImage("coconut.png")) ?? CreateIngredientFallbackImage("Coconut");

            playerLeftImage = Services.AssetLoader.LoadImage("a - left.png", "a-left.png", "A-left.png");
            playerRightImage = Services.AssetLoader.LoadImage("d - right.png", "d -right.png", "d-right.png", "D-right.png");
            playerDownImage = Services.AssetLoader.LoadImage("S-down.png", "s-down.png");
            playerUpImage = Services.AssetLoader.LoadImage("w-up.png", "W-up.png");

            // Captain sprite (optional). Put captain.png into assets folder.
            captainImage = Services.AssetLoader.LoadImage("captain.png");
        }

        private void StyleButtons()
        {
            StyleButton(btnBackToMenu, "#0ea5e9", "#38bdf8");
            StyleButton(btnSaveGame, "#16a34a", "#22c55e");
            StyleButton(btnLoadGame, "#f59e0b", "#fbbf24");
        }

        private void StyleButton(Button button, string normalHex, string hoverHex)
        {
            var normalColor = ColorTranslator.FromHtml(normalHex);
            var hoverColor = ColorTranslator.FromHtml(hoverHex);

            button.BackColor = normalColor;
            button.FlatAppearance.BorderSize = 0;
            button.MouseEnter += (s, e) => button.BackColor = hoverColor;
            button.MouseLeave += (s, e) => button.BackColor = normalColor;
            button.Resize += (s, e) =>
            {
                using (var path = new GraphicsPath())
                {
                    var radius = 12;
                    path.AddArc(0, 0, radius, radius, 180, 90);
                    path.AddArc(button.Width - radius, 0, radius, radius, 270, 90);
                    path.AddArc(button.Width - radius, button.Height - radius, radius, radius, 0, 90);
                    path.AddArc(0, button.Height - radius, radius, radius, 90, 90);
                    path.CloseAllFigures();
                    button.Region = new Region(path);
                }
            };
        }

        private void CreateMapLayout()
        {
            panelMap.Controls.Clear();
            wallBlocks.Clear();
            trashSpawnAreas.Clear();

            var selectedMapImage = currentMapKey == "map2" ? map2BackgroundImage : mapBackgroundImage;
            var hasMapImage = selectedMapImage != null;
            var layout = Services.MapLayoutFactory.Create(currentMapKey, hasMapImage, panelMap.Size);

            panelMap.BackgroundImage = selectedMapImage;
            panelMap.BackgroundImageLayout = hasMapImage ? ImageLayout.Stretch : ImageLayout.None;

            if (currentMapKey == "map1" && !hasMapImage)
            {
                var street = new Panel
                {
                    BackColor = ColorTranslator.FromHtml("#1e293b"),
                    Bounds = new Rectangle(0, panelMap.Height / 2 - 60, panelMap.Width, 120)
                };
                panelMap.Controls.Add(street);

                CreateBuilding("Barangay Hall", new Rectangle(80, 90, 240, 150), "#334155");
                CreateBuilding("House A", new Rectangle(430, 95, 170, 120), "#475569");
                CreateBuilding("House B", new Rectangle(640, 95, 170, 120), "#475569");
                CreateBuilding("House C", new Rectangle(860, 95, 170, 120), "#475569");
                CreateBuilding("Sari-Sari Store", new Rectangle(860, 620, 200, 130), "#334155");

                var plaza = new Panel
                {
                    BackColor = ColorTranslator.FromHtml("#0b3b2f"),
                    Bounds = new Rectangle(80, 560, 400, 200)
                };
                panelMap.Controls.Add(plaza);
            }

            foreach (var wallBounds in layout.WallBounds)
            {
                AddInvisibleWall(wallBounds);
            }

            trashSpawnAreas.AddRange(layout.TrashSpawnAreas);
            storeInteractionZone = layout.StoreInteractionZone;
            currentMapAllowsTrash = layout.AllowTrash;

            mapDownTransitionZone = currentMapKey == "map1"
                ? new Rectangle(0, Math.Max(0, panelMap.Height - 28), panelMap.Width, 28)
                : Rectangle.Empty;

            mapUpTransitionZone = currentMapKey == "map2"
                ? new Rectangle(0, 0, panelMap.Width, 28)
                : Rectangle.Empty;

            captainNpc = null;
            if (layout.HasCaptainNpc)
            {
                var hallBounds = layout.HallBounds;

                captainNpc = new PictureBox
                {
                    BackColor = ColorTranslator.FromHtml("#38bdf8"),
                    Size = new Size(42, 58),
                    Tag = "Captain"
                };

                if (captainImage != null)
                {
                    captainNpc.Size = new Size(54, 72);
                    captainNpc.BackColor = Color.Transparent;
                    captainNpc.SizeMode = PictureBoxSizeMode.Zoom;
                    captainNpc.Image = captainImage;
                }

                var capX = hallBounds.Left + (hallBounds.Width / 2) - (captainNpc.Width / 2);
                var capY = hallBounds.Top + hallBounds.Height - captainNpc.Height - 8;
                captainNpc.Location = new Point(capX, capY);

                var colliderBounds = new Rectangle(captainNpc.Left, captainNpc.Top, captainNpc.Width, captainNpc.Height);
                colliderBounds.Inflate(8, 10);
                var npcCollider = new Panel
                {
                    Bounds = colliderBounds,
                    Visible = false,
                    Tag = "Wall"
                };

                panelMap.Controls.Add(npcCollider);
                wallBlocks.Add(npcCollider);

                panelMap.Controls.Add(captainNpc);
                captainNpc.BringToFront();
            }

            playerSprite = new PictureBox
            {
                BackColor = ColorTranslator.FromHtml("#22c55e"),
                Size = new Size(36, 36),
                Location = hasMapImage ? layout.PlayerSpawnWithMapImage : layout.PlayerSpawnFallback,
                Tag = "Player"
            };

            if (playerUpImage != null || playerDownImage != null || playerLeftImage != null || playerRightImage != null)
            {
                playerSprite.Size = new Size(54, 72);
                playerSprite.Location = hasMapImage ? layout.PlayerSpawnWithMapImage : layout.PlayerSpawnFallback;
                playerSprite.BackColor = Color.Transparent;
                playerSprite.SizeMode = PictureBoxSizeMode.Zoom;
                ApplyPlayerSpriteImage("down");
            }

            if (hasLoadedProgress)
            {
                var savedX = Math.Max(0, Math.Min(panelMap.Width - playerSprite.Width, loadedPlayerLocation.X));
                var savedY = Math.Max(0, Math.Min(panelMap.Height - playerSprite.Height, loadedPlayerLocation.Y));
                playerSprite.Location = new Point(savedX, savedY);
            }

            panelMap.Controls.Add(playerSprite);
            playerSprite.BringToFront();
            EnsureFloatingHintLabel();

            nearCaptain = false;
            nearStore = false;
            playerX = playerSprite.Left;
            playerY = playerSprite.Top;
        }

        private void EnsureFloatingHintLabel()
        {
            if (floatingHintLabel != null)
            {
                return;
            }

            floatingHintLabel = new Label
            {
                AutoSize = true,
                Visible = false,
                BackColor = Color.FromArgb(170, 15, 23, 42),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Padding = new Padding(8, 4, 8, 4)
            };

            panelMap.Controls.Add(floatingHintLabel);
            floatingHintLabel.BringToFront();
        }

        private void CreateBuilding(string name, Rectangle bounds, string colorHex)
        {
            var building = new Panel
            {
                Bounds = bounds,
                BackColor = ColorTranslator.FromHtml(colorHex),
                Tag = "Wall"
            };

            var nameLabel = new Label
            {
                Dock = DockStyle.Fill,
                Text = name,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.Transparent
            };

            building.Controls.Add(nameLabel);
            panelMap.Controls.Add(building);
            wallBlocks.Add(building);
        }

        private void AddInvisibleWall(Rectangle bounds)
        {
            var wall = new Panel
            {
                Bounds = bounds,
                Visible = false,
                Tag = "Wall"
            };

            panelMap.Controls.Add(wall);
            wallBlocks.Add(wall);
        }

        private void SpawnTrash(int count)
        {
            var maxAttempts = 500;
            var attempts = 0;

            while (trashItems.Count < count && attempts < maxAttempts)
            {
                attempts++;

                var trash = new PictureBox
                {
                    Size = new Size(40, 40)
                };

                if (trashImage != null)
                {
                    trash.Image = trashImage;
                    trash.SizeMode = PictureBoxSizeMode.Zoom;
                    trash.BackColor = Color.Transparent;
                }
                else
                {
                    trash.BackColor = ColorTranslator.FromHtml("#94a3b8");
                }

                var position = GetTrashSpawnPosition(trash.Size);
                trash.Location = position;

                var trashBounds = trash.Bounds;
                var isInvalid = wallBlocks.Any(w => w.Bounds.IntersectsWith(trashBounds)) ||
                                (captainNpc != null && trashBounds.IntersectsWith(captainNpc.Bounds)) ||
                                trashBounds.IntersectsWith(playerSprite.Bounds) ||
                                trashItems.Any(t => t.Sprite.Bounds.IntersectsWith(trashBounds));

                if (isInvalid)
                {
                    continue;
                }

                panelMap.Controls.Add(trash);
                trash.BringToFront();
                if (captainNpc != null)
                {
                    captainNpc.BringToFront();
                }
                playerSprite.BringToFront();
                trashItems.Add(new TrashItem(trash));
            }
        }

        private Point GetTrashSpawnPosition(Size trashSize)
        {
            if (trashSpawnAreas.Count == 0)
            {
                var xFallback = random.Next(30, Math.Max(31, panelMap.Width - trashSize.Width - 30));
                var yFallback = random.Next(30, Math.Max(31, panelMap.Height - trashSize.Height - 30));
                return new Point(xFallback, yFallback);
            }

            var zone = trashSpawnAreas[random.Next(trashSpawnAreas.Count)];
            var xMax = Math.Max(zone.Left + 1, zone.Right - trashSize.Width);
            var yMax = Math.Max(zone.Top + 1, zone.Bottom - trashSize.Height);
            var x = random.Next(zone.Left, xMax);
            var y = random.Next(zone.Top, yMax);

            return new Point(x, y);
        }

        private int GetTrashSpawnTarget(string mapKey = null)
        {
            var key = mapKey ?? currentMapKey;
            var baseCount = key == "map2" ? 12 : 18;
            var reduction = wasteBinLevel * 2;
            return Math.Max(6, baseCount - reduction);
        }

        private void ApplyPlayerSpriteImage(string direction)
        {
            playerFacing = direction;

            Image selected = null;
            switch (direction)
            {
                case "up":
                    selected = playerUpImage;
                    break;
                case "left":
                    selected = playerLeftImage;
                    break;
                case "right":
                    selected = playerRightImage;
                    break;
                default:
                    selected = playerDownImage;
                    break;
            }

            if (selected != null && !ReferenceEquals(playerSprite.Image, selected))
            {
                playerSprite.Image = selected;
            }
        }

        // Game loop - dito nangyayari yung bawat frame ng game
        private void gameTimer_Tick(object sender, EventArgs e)
        {
            if (interactionCooldownFrames > 0)
            {
                interactionCooldownFrames--;
            }

            var moved = MovePlayer();
            HandleStamina(moved);
            UpdateDayCycle();
            HandleMapTransition();
            HandleTrashCollection();
            HandleIngredientCollection();
            UpdateDailyMissionProgress();
            HandleStoreInteraction();
            UpdateNearbyNpcState();
            UpdateFloatingHintPosition();
            UpdateHud();
        }

        private void UpdateFloatingHintPosition()
        {
            if (floatingHintLabel == null || !floatingHintLabel.Visible || playerSprite == null)
            {
                return;
            }

            var x = playerSprite.Left + ((playerSprite.Width - floatingHintLabel.Width) / 2);
            var y = playerSprite.Top - floatingHintLabel.Height - 10;

            x = Math.Max(6, Math.Min(panelMap.Width - floatingHintLabel.Width - 6, x));
            y = Math.Max(6, y);

            floatingHintLabel.Location = new Point(x, y);
            floatingHintLabel.BringToFront();
        }

        private void SetFloatingHint(string text)
        {
            EnsureFloatingHintLabel();

            if (string.IsNullOrWhiteSpace(text))
            {
                floatingHintLabel.Visible = false;
                return;
            }

            if (!string.Equals(floatingHintLabel.Text, text, StringComparison.Ordinal))
            {
                floatingHintLabel.Text = text;
            }

            floatingHintLabel.Visible = true;
            UpdateFloatingHintPosition();
        }

        private void ClearMovementInput()
        {
            moveUp = false;
            moveDown = false;
            moveLeft = false;
            moveRight = false;
        }

        private bool CanInteract()
        {
            return !interactionLocked && interactionCooldownFrames == 0;
        }

        private void LockInteraction()
        {
            interactionLocked = true;
            interactionCooldownFrames = InteractionCooldownFrames;
        }

        // Map transition - pag lumabas sa edge ng map, lipat sa kabilang map
        private void HandleMapTransition()
        {
            var playerBounds = GetPlayerCollisionBounds(playerX, playerY);

            if (currentMapKey == "map1" && moveDown && !mapDownTransitionZone.IsEmpty && playerBounds.IntersectsWith(mapDownTransitionZone))
            {
                SwitchMap("map2", new Point(Math.Max(10, (panelMap.Width / 2) - (playerSprite.Width / 2)), 24));
            }
            else if (currentMapKey == "map2" && moveUp && !mapUpTransitionZone.IsEmpty && playerBounds.IntersectsWith(mapUpTransitionZone))
            {
                SwitchMap("map1", new Point(Math.Max(10, (panelMap.Width / 2) - (playerSprite.Width / 2)), Math.Max(30, panelMap.Height - playerSprite.Height - 60)));
            }
        }

        private void HandleStoreInteraction()
        {
            var interactionZone = GetPlayerCollisionBounds(playerX, playerY);
            interactionZone.Inflate(16, 16);

            nearStore = !storeInteractionZone.IsEmpty && interactionZone.IntersectsWith(storeInteractionZone);
        }

        private void ShowStoreModal()
        {
            isStoreModalOpen = true;
            gameTimer.Stop();
            moveUp = false;
            moveDown = false;
            moveLeft = false;
            moveRight = false;

            using (var storeDialog = new StoreDialog(
                () => btnBuyGloves_Click(this, EventArgs.Empty),
                () => btnBuyEnergyDrink_Click(this, EventArgs.Empty),
                () => btnBuyTrashBag_Click(this, EventArgs.Empty)))
            {
                storeDialog.ShowDialog(this);
            }

            isStoreModalOpen = false;
            gameTimer.Start();
        }

        // Movement logic - may collision detection para hindi tumusok sa walls
        private bool MovePlayer()
        {
            if (player.Stamina <= 0)
            {
                return false;
            }

            var directionX = 0;
            var directionY = 0;

            if (moveUp) directionY--;
            if (moveDown) directionY++;
            if (moveLeft) directionX--;
            if (moveRight) directionX++;

            if (directionX == 0 && directionY == 0)
            {
                return false;
            }

            var magnitude = Math.Sqrt((directionX * directionX) + (directionY * directionY));
            var velocityX = (directionX / magnitude) * player.Speed;
            var velocityY = (directionY / magnitude) * player.Speed;

            var moved = false;

            var nextX = playerX + velocityX;
            var candidateX = GetPlayerCollisionBounds(nextX, playerY);
            if (IsInsideMap(candidateX) && !IsCollidingWithWalls(candidateX))
            {
                playerX = nextX;
                moved = true;
            }

            var nextY = playerY + velocityY;
            var candidateY = GetPlayerCollisionBounds(playerX, nextY);
            if (IsInsideMap(candidateY) && !IsCollidingWithWalls(candidateY))
            {
                playerY = nextY;
                moved = true;
            }

            var roundedX = (int)Math.Round(playerX);
            var roundedY = (int)Math.Round(playerY);

            if (playerSprite.Left != roundedX || playerSprite.Top != roundedY)
            {
                playerSprite.Location = new Point(roundedX, roundedY);
            }

            return moved;
        }

        private Rectangle GetPlayerCollisionBounds(double spriteLeft, double spriteTop)
        {
            var left = (int)Math.Round(spriteLeft + (playerSprite.Width * 0.2));
            var top = (int)Math.Round(spriteTop + (playerSprite.Height * 0.62));
            var width = Math.Max(12, (int)Math.Round(playerSprite.Width * 0.6));
            var height = Math.Max(12, (int)Math.Round(playerSprite.Height * 0.34));
            return new Rectangle(left, top, width, height);
        }

        private Point GetPlayerFeetPoint()
        {
            var bounds = GetPlayerCollisionBounds(playerX, playerY);
            return new Point(bounds.Left + (bounds.Width / 2), bounds.Bottom);
        }

        private bool IsInsideMap(Rectangle bounds)
        {
            return bounds.Left >= 0 &&
                   bounds.Top >= 0 &&
                   bounds.Right <= panelMap.Width &&
                   bounds.Bottom <= panelMap.Height;
        }

        private bool IsCollidingWithWalls(Rectangle bounds)
        {
            for (var i = 0; i < wallBlocks.Count; i++)
            {
                if (wallBlocks[i].Bounds.IntersectsWith(bounds))
                {
                    return true;
                }
            }

            return false;
        }

        private void HandleStamina(bool moved)
        {
            moveFrameCounter++;

            if (moved)
            {
                if (moveFrameCounter % 6 == 0)
                {
                    player.ConsumeStamina(1);
                }
            }
            else
            {
                if (moveFrameCounter % 5 == 0)
                {
                    player.RecoverStamina(1 + streetlightLevel);
                }
            }
        }

        private void OpenCommunityProjectsMenu()
        {
            var summary = "Barangay Fund: ₱" + barangayFund + " / ₱" + BarangayFundGoal + "\n\n" +
                          "Choose a project upgrade:\n" +
                          "Yes = Streetlights (₱220) [Level " + streetlightLevel + "]\n" +
                          "No = Waste Bins (₱180) [Level " + wasteBinLevel + "]\n" +
                          "Cancel = Community Garden (₱260) [Level " + gardenLevel + "]";

            var decision = BayanihanMessageBox.Show(this, summary, "Community Projects", BayanihanMessageType.Confirmation, MessageBoxButtons.YesNoCancel);

            if (decision == DialogResult.Yes)
            {
                TryUpgradeProject("Streetlights", 220, ref streetlightLevel);
            }
            else if (decision == DialogResult.No)
            {
                TryUpgradeProject("Waste Bins", 180, ref wasteBinLevel);
            }
            else if (decision == DialogResult.Cancel)
            {
                TryUpgradeProject("Community Garden", 260, ref gardenLevel);
            }
        }

        private void TryUpgradeProject(string projectName, int cost, ref int level)
        {
            if (!player.TrySpend(cost))
            {
                BayanihanMessageBox.Show(this, "Not enough money for " + projectName + ".", "Projects", BayanihanMessageType.Warning, MessageBoxButtons.OK);
                return;
            }

            level++;
            AddFund(80 + (level * 10));
            BayanihanMessageBox.Show(this, projectName + " upgraded to Level " + level + ".", "Projects", BayanihanMessageType.Success, MessageBoxButtons.OK);
            UpdateHud();
        }

        private void TryOpenNpcRequest()
        {
            if (inGameDay == lastNpcRequestDay)
            {
                BayanihanMessageBox.Show(this, "You already handled an NPC request today. Try again next day.", "NPC Request", BayanihanMessageType.Info, MessageBoxButtons.OK);
                return;
            }

            var request = "Resident Request:\nA family needs support.\n\nYes: Deliver relief package (+Rep)\nNo: Join drainage cleanup (+Money)\nCancel: Tutor youth on waste segregation (+Balanced)";
            var choice = BayanihanMessageBox.Show(this, request, "Barangay Request Board", BayanihanMessageType.Confirmation, MessageBoxButtons.YesNoCancel);

            if (choice == DialogResult.Yes)
            {
                GrantReward(80, 14);
                AddFund(40);
            }
            else if (choice == DialogResult.No)
            {
                GrantReward(140, 7);
                AddFund(55);
            }
            else if (choice == DialogResult.Cancel)
            {
                GrantReward(100, 10);
                AddFund(50);
            }
            else
            {
                return;
            }

            lastNpcRequestDay = inGameDay;
            BayanihanMessageBox.Show(this, "NPC request completed for Day " + inGameDay + ".", "NPC Request", BayanihanMessageType.Success, MessageBoxButtons.OK);
        }

        private void OpenMiniGameHub()
        {
            var intro = "Mini-Game Hub:\nYes = Waste Sorting Quiz\nNo = Budget Allocation Quiz";
            var choice = BayanihanMessageBox.Show(this, intro, "CWTS Mini-Games", BayanihanMessageType.Confirmation, MessageBoxButtons.YesNo);

            if (choice == DialogResult.Yes)
            {
                PlayWasteSortingMiniGame();
            }
            else if (choice == DialogResult.No)
            {
                PlayBudgetAllocationMiniGame();
            }
        }

        private void PlayWasteSortingMiniGame()
        {
            var score = 0;
            if (BayanihanMessageBox.Show(this, "Plastic bottle goes to recyclable bin?", "Sorting Quiz", BayanihanMessageType.Confirmation, MessageBoxButtons.YesNo) == DialogResult.Yes) score++;
            if (BayanihanMessageBox.Show(this, "Food leftovers are residual waste?", "Sorting Quiz", BayanihanMessageType.Confirmation, MessageBoxButtons.YesNo) == DialogResult.No) score++;
            if (BayanihanMessageBox.Show(this, "Dry leaves are biodegradable waste?", "Sorting Quiz", BayanihanMessageType.Confirmation, MessageBoxButtons.YesNo) == DialogResult.Yes) score++;

            var rewardMoney = 60 + (score * 25);
            var rewardRep = 5 + (score * 3);
            miniGamesCompleted++;
            GrantReward(rewardMoney, rewardRep);
            AddFund(35 + (score * 5));

            BayanihanMessageBox.Show(this, "Sorting score: " + score + "/3\nReward: +P" + rewardMoney + ", +" + rewardRep + " Reputation", "Mini-Game Result", BayanihanMessageType.Success, MessageBoxButtons.OK);
        }

        private void PlayBudgetAllocationMiniGame()
        {
            var scenario = "Allocate barangay budget:\nYes = 50% Flood Prep, 30% Health, 20% Youth\nNo = 15% Flood Prep, 70% Festival, 15% Health";
            var choice = BayanihanMessageBox.Show(this, scenario, "Budget Quiz", BayanihanMessageType.Confirmation, MessageBoxButtons.YesNo);
            var goodPlan = choice == DialogResult.Yes;

            miniGamesCompleted++;
            if (goodPlan)
            {
                GrantReward(140, 16);
                AddFund(70);
                BayanihanMessageBox.Show(this, "Great allocation. You prioritized risk reduction and core services.", "Budget Quiz", BayanihanMessageType.Success, MessageBoxButtons.OK);
            }
            else
            {
                GrantReward(70, 6);
                AddFund(30);
                BayanihanMessageBox.Show(this, "Allocation was weak. Community impact is lower.", "Budget Quiz", BayanihanMessageType.Warning, MessageBoxButtons.OK);
            }
        }

        // Day cycle system - nag-a-update ng araw at daily missions
        private void UpdateDayCycle()
        {
            dayFrameCounter++;
            if (dayFrameCounter < FramesPerInGameDay)
            {
                return;
            }

            dayFrameCounter = 0;
            if (!dailyMissionCompleted)
            {
                dailyMissionStreak = 0;
            }
            inGameDay++;
            dailyTrashTarget = Math.Min(18, 8 + ((inGameDay - 1) / 2));
            dailyTrashProgress = 0;
            dailyMissionCompleted = false;
            if (communityEventActive && inGameDay > communityEventEndDay)
            {
                communityEventActive = false;
                communityEventName = string.Empty;
                communityEventGoal = 0;
                communityEventProgress = 0;
            }

            StartCommunityEventIfNeeded();

            ShowGameplayPopup(
                "Day " + inGameDay + " has started.\nNew daily mission: collect " + dailyTrashTarget + " trash items.",
                "New Day",
                BayanihanMessageType.Info);
        }

        private void StartCommunityEventIfNeeded()
        {
            if (communityEventActive || inGameDay < 3 || inGameDay % 3 != 0)
            {
                return;
            }

            var roll = random.Next(3);
            communityEventActive = true;
            communityEventProgress = 0;
            communityEventEndDay = inGameDay + 1;

            if (roll == 0)
            {
                communityEventName = "Flood Preparedness Cleanup";
                communityEventGoal = 12;
            }
            else if (roll == 1)
            {
                communityEventName = "Emergency Pantry Packing";
                communityEventGoal = 10;
            }
            else
            {
                communityEventName = "Volunteer Mobilization";
                communityEventGoal = 1;
            }

            ShowGameplayPopup(
                "Community Event Started: " + communityEventName + "\nComplete before Day " + communityEventEndDay + ".",
                "Community Event",
                BayanihanMessageType.Info);
        }

        private void TryCompleteCommunityEvent()
        {
            if (!communityEventActive || communityEventProgress < communityEventGoal)
            {
                return;
            }

            communityEventActive = false;
            communityEventsCompleted++;
            var rewardMoney = 220 + (communityEventsCompleted * 20);
            var rewardRep = 16 + Math.Min(12, communityEventsCompleted * 2);
            GrantReward(rewardMoney, rewardRep);
            AddFund(120);

            ShowGameplayPopup(
                "Community Event Completed: " + communityEventName + "\nReward: +P" + rewardMoney + ", +" + rewardRep + " Reputation",
                "Event Success",
                BayanihanMessageType.Success);

            communityEventName = string.Empty;
            communityEventGoal = 0;
            communityEventProgress = 0;
        }

        private void UpdateDailyMissionProgress()
        {
            if (dailyMissionCompleted)
            {
                return;
            }

            if (dailyTrashProgress < dailyTrashTarget)
            {
                return;
            }

            dailyMissionCompleted = true;
            dailyMissionStreak++;
            var streakBonus = Math.Min(60, dailyMissionStreak * 10);
            var rewardMoney = 120 + streakBonus;
            var rewardReputation = 8 + Math.Min(8, dailyMissionStreak);

            GrantReward(rewardMoney, rewardReputation);
            AddFund(90 + (dailyMissionStreak * 5));
            if (communityEventActive && string.Equals(communityEventName, "Volunteer Mobilization", StringComparison.Ordinal))
            {
                communityEventProgress = 1;
            }
            TryCompleteCommunityEvent();

            ShowGameplayPopup(
                "Daily mission complete!\nStreak: " + dailyMissionStreak + "\nReward: +P" + rewardMoney + ", +" + rewardReputation + " Reputation",
                "Daily Mission",
                BayanihanMessageType.Success);
        }

        private void AddFund(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            barangayFund += amount;
            if (barangayFund >= BarangayFundGoal)
            {
                barangayFund = BarangayFundGoal;
                if (!fundGoalAnnounced)
                {
                    fundGoalAnnounced = true;
                    player.AddReward(250, 20);
                    ShowGameplayPopup(
                        "Barangay Fund Goal reached!\nYou unlocked a development bonus for strong financial stewardship.\nReward: +P250, +20 Reputation",
                        "Fund Goal Complete",
                        BayanihanMessageType.Success);
                }
            }
        }

        private void GrantReward(int money, int reputation)
        {
            player.AddReward(money, reputation);
            var fundShare = Math.Max(10, (int)Math.Round(money * 0.35));
            AddFund(fundShare);
        }

        // Trash collection - kinukuha yung trash pag malapit na sa player
        private void HandleTrashCollection()
        {
            var currentQuest = GetCurrentMapQuest();
            if (!currentMapAllowsTrash || currentQuest.Status != QuestStatus.InProgress)
            {
                return;
            }

            var pickupBounds = playerSprite.Bounds;
            pickupBounds.Inflate(18, 18);
            var playerCenter = new Point(
                playerSprite.Left + (playerSprite.Width / 2),
                playerSprite.Top + (playerSprite.Height / 2));

            for (var i = trashItems.Count - 1; i >= 0; i--)
            {
                var trash = trashItems[i];

                if (trash.IsCollected)
                {
                    continue;
                }

                var trashCenter = new Point(
                    trash.Sprite.Left + (trash.Sprite.Width / 2),
                    trash.Sprite.Top + (trash.Sprite.Height / 2));

                var dx = playerCenter.X - trashCenter.X;
                var dy = playerCenter.Y - trashCenter.Y;
                var isNearTrash = (dx * dx) + (dy * dy) <= TrashCollectRadiusSquared;

                if (!pickupBounds.IntersectsWith(trash.Sprite.Bounds) && !isNearTrash)
                {
                    continue;
                }

                if (player.TrashCapacity <= player.TrashCollected)
                {
                    SetHintText("Bag is full. Buy Trash Bag upgrade in the store.");
                    break;
                }

                trash.Collect();
                panelMap.Controls.Remove(trash.Sprite);
                trash.Sprite.Dispose();
                trashItems.RemoveAt(i);

                player.TrashCollected++;
                player.TotalTrashCollected++;
                dailyTrashProgress++;
                if (communityEventActive && string.Equals(communityEventName, "Flood Preparedness Cleanup", StringComparison.Ordinal))
                {
                    communityEventProgress++;
                }
                AddFund(6 + wasteBinLevel);
                player.ConsumeStamina(player.HasGloves ? 1 : 2);

                if (currentQuest.Status == QuestStatus.InProgress)
                {
                    currentQuest.AddProgress(1);
                }
            }

            if (trashItems.Count <= 6)
            {
                SpawnTrash(GetTrashSpawnTarget());
            }

            TryCompleteCommunityEvent();
        }

        private void SpawnIngredients(int count)
        {
            if (currentMapKey != "map1" || !pantryMissionInProgress || pantryMissionCompleted)
            {
                return;
            }

            var maxAttempts = 500;
            var attempts = 0;
            var ingredientTypes = new[] { "Chicken", "Coconut" };

            while (ingredientItems.Count < count && attempts < maxAttempts)
            {
                attempts++;
                var ingredientType = ingredientTypes[random.Next(ingredientTypes.Length)];
                var item = new PictureBox
                {
                    Size = new Size(40, 40),
                    Tag = ingredientType
                };

                var icon = ResolveIngredientIcon(ingredientType);
                if (icon != null)
                {
                    item.Image = icon;
                    item.SizeMode = PictureBoxSizeMode.Zoom;
                    item.BackColor = Color.Transparent;
                }
                else
                {
                    item.BackColor = string.Equals(ingredientType, "Chicken", StringComparison.Ordinal)
                        ? ColorTranslator.FromHtml("#f59e0b")
                        : ColorTranslator.FromHtml("#84cc16");
                }

                item.Location = GetTrashSpawnPosition(item.Size);
                var bounds = item.Bounds;
                var isInvalid = wallBlocks.Any(w => w.Bounds.IntersectsWith(bounds)) ||
                                (captainNpc != null && bounds.IntersectsWith(captainNpc.Bounds)) ||
                                bounds.IntersectsWith(playerSprite.Bounds) ||
                                trashItems.Any(t => t.Sprite.Bounds.IntersectsWith(bounds)) ||
                                ingredientItems.Any(i => i.Sprite.Bounds.IntersectsWith(bounds));

                if (isInvalid)
                {
                    continue;
                }

                panelMap.Controls.Add(item);
                item.BringToFront();
                if (captainNpc != null)
                {
                    captainNpc.BringToFront();
                }
                playerSprite.BringToFront();
                ingredientItems.Add(new IngredientItem(item, ingredientType));
            }
        }

        private Image ResolveIngredientIcon(string ingredientType)
        {
            if (string.Equals(ingredientType, "Chicken", StringComparison.Ordinal))
            {
                return chickenImage ?? CreateIngredientFallbackImage("Chicken");
            }

            if (string.Equals(ingredientType, "Coconut", StringComparison.Ordinal))
            {
                return coconutImage ?? CreateIngredientFallbackImage("Coconut");
            }

            return null;
        }

        private void HandleIngredientCollection()
        {
            if (currentMapKey != "map1" || !pantryMissionInProgress || pantryMissionCompleted)
            {
                return;
            }

            var pickupBounds = playerSprite.Bounds;
            pickupBounds.Inflate(18, 18);
            var playerCenter = new Point(
                playerSprite.Left + (playerSprite.Width / 2),
                playerSprite.Top + (playerSprite.Height / 2));

            for (var i = ingredientItems.Count - 1; i >= 0; i--)
            {
                var ingredient = ingredientItems[i];
                if (ingredient.IsCollected)
                {
                    continue;
                }

                var itemCenter = new Point(
                    ingredient.Sprite.Left + (ingredient.Sprite.Width / 2),
                    ingredient.Sprite.Top + (ingredient.Sprite.Height / 2));
                var dx = playerCenter.X - itemCenter.X;
                var dy = playerCenter.Y - itemCenter.Y;
                var near = (dx * dx) + (dy * dy) <= IngredientCollectRadiusSquared;

                if (!pickupBounds.IntersectsWith(ingredient.Sprite.Bounds) && !near)
                {
                    continue;
                }

                ingredient.Collect();
                panelMap.Controls.Remove(ingredient.Sprite);
                ingredient.Sprite.Dispose();
                ingredientItems.RemoveAt(i);

                if (string.Equals(ingredient.IngredientType, "Chicken", StringComparison.Ordinal))
                {
                    chickenCollected++;
                }
                else
                {
                    coconutCollected++;
                }

                if (communityEventActive && string.Equals(communityEventName, "Emergency Pantry Packing", StringComparison.Ordinal))
                {
                    communityEventProgress++;
                }

                AddFund(4 + gardenLevel);
            }

            if (ingredientItems.Count <= 3)
            {
                SpawnIngredients(8);
            }

            TryCompleteCommunityEvent();
        }

        private void UpdateNearbyNpcState()
        {
            var interactionZone = GetPlayerCollisionBounds(playerX, playerY);
            interactionZone.Inflate(22, 22);

            nearCaptain = captainNpc != null && interactionZone.IntersectsWith(captainNpc.Bounds);

            if (nearCaptain)
            {
                SetHintText("Press E to talk to Barangay Captain");
                SetFloatingHint("Press E to talk to Barangay Captain");
            }
            else if (nearStore)
            {
                SetHintText("Press E to open the shop");
                SetFloatingHint("Press E to open the shop");
            }
            else if (currentMapKey == "map2")
            {
                if (map2Quest.Status == QuestStatus.NotStarted)
                {
                    SetHintText("Press E to accept Road Cleanup quest");
                    SetFloatingHint(string.Empty);
                }
                else if (map2Quest.Status == QuestStatus.Completed)
                {
                    SetHintText("Press E to claim Road Cleanup reward");
                    SetFloatingHint(string.Empty);
                }
                else
                {
                    SetHintText("Road Cleanup in progress. Collect trash on the road.");
                    SetFloatingHint(string.Empty);
                }
            }
            else if (player.Stamina <= 0)
            {
                SetHintText("You are tired. Buy Energy Drink or wait to recover stamina.");
                SetFloatingHint(string.Empty);
            }
            else if (currentMapKey == "map1" && pantryMissionInProgress && !pantryMissionCompleted)
            {
                SetHintText("CWTS: Collect Chicken and Coconut for the Community Pantry.");
                SetFloatingHint(string.Empty);
            }
            else
            {
                SetHintText("Move with W A S D | P: Projects | N: NPC Request | M: Mini-Games");
                SetFloatingHint(string.Empty);
            }
        }

        private void SetHintText(string text)
        {
            if (!string.Equals(lblHint.Text, text, StringComparison.Ordinal))
            {
                lblHint.Text = text;
            }
        }

        private void ShowGameplayPopup(string message, string title, BayanihanMessageType type)
        {
            var wasRunning = gameTimer != null && gameTimer.Enabled;
            if (wasRunning)
            {
                gameTimer.Stop();
            }

            ClearMovementInput();
            interactionLocked = true;

            try
            {
                BayanihanMessageBox.Show(this, message, title, type, MessageBoxButtons.OK);
            }
            finally
            {
                ClearMovementInput();
                interactionLocked = false;
                if (wasRunning)
                {
                    gameTimer.Start();
                }
            }
        }

        // NPC interaction - dito yung quest system at mission selection
        private void HandleCaptainInteraction()
        {
            if (!nearCaptain)
            {
                return;
            }

            if (pantryMissionUnlocked)
            {
                if (pantryMissionInProgress && !pantryMissionCompleted)
                {
                    if (chickenCollected >= PantryChickenGoal && coconutCollected >= PantryCoconutGoal)
                    {
                        pantryMissionCompleted = true;
                        pantryMissionInProgress = false;
                        GrantReward(300, 30);
                        ClearIngredientItems();
                        ShowGameplayPopup(
                            "Community Pantry Drive completed!\n\nYou helped prepare relief ingredients for families in need.\nReward: +P300, +30 Reputation",
                            "CWTS Success",
                            BayanihanMessageType.Success);
                    }
                    else
                    {
                        BayanihanMessageBox.ShowQuestDialogue(
                            this,
                            "Barangay Captain",
                            "CWTS Mission: Community Pantry Drive\nCollect at least " + PantryChickenGoal + " Chicken and " + PantryCoconutGoal + " Coconut for relief meal packs.\nCurrent: Chicken " + chickenCollected + "/" + PantryChickenGoal + ", Coconut " + coconutCollected + "/" + PantryCoconutGoal,
                            "CWTS Mission");
                        return;
                    }
                }

                var missionPick = BayanihanMessageBox.Show(
                    this,
                    "Mission Board (Repeatable)\n\nYes: Barangay Cleanup (Map 1)\nNo: Community Pantry Drive (Map 1)\nCancel: Road Cleanup (Map 2)",
                    "Choose Mission",
                    BayanihanMessageType.Confirmation,
                    MessageBoxButtons.YesNoCancel);

                if (missionPick == DialogResult.Yes)
                {
                    pantryMissionInProgress = false;
                    pantryMissionCompleted = false;
                    activeQuest.LoadState(QuestStatus.InProgress, 0);
                    player.TrashCollected = 0;
                    ClearTrashItems();
                    if (currentMapKey == "map1")
                    {
                        SpawnTrash(GetTrashSpawnTarget("map1"));
                    }
                    ShowGameplayPopup("Cleanup mission started. Collect trash and return for reward.", "Mission Started", BayanihanMessageType.Info);
                    return;
                }

                if (missionPick == DialogResult.No)
                {
                    pantryMissionInProgress = true;
                    pantryMissionCompleted = false;
                    chickenCollected = 0;
                    coconutCollected = 0;
                    ClearIngredientItems();
                    if (currentMapKey == "map1")
                    {
                        SpawnIngredients(10);
                    }
                    ShowGameplayPopup("Community Pantry mission started. Gather Chicken and Coconut.", "Mission Started", BayanihanMessageType.Info);
                    return;
                }

                if (missionPick == DialogResult.Cancel)
                {
                    map2Quest.LoadState(QuestStatus.InProgress, 0);
                    ClearTrashItems();
                    if (currentMapKey == "map2")
                    {
                        SpawnTrash(GetTrashSpawnTarget("map2"));
                        ShowGameplayPopup("Road Cleanup mission started on Map 2.", "Mission Started", BayanihanMessageType.Info);
                    }
                    else
                    {
                        ShowGameplayPopup("Road Cleanup mission started. Go to Map 2 to collect road trash.", "Mission Started", BayanihanMessageType.Info);
                    }

                    return;
                }

                return;
            }

            if (activeQuest.Status == QuestStatus.NotStarted)
            {
                activeQuest.Start();
                ClearTrashItems();
                ClearIngredientItems();
                SpawnTrash(GetTrashSpawnTarget("map1"));
                BayanihanMessageBox.ShowQuestDialogue(
                    this,
                    "Barangay Captain",
                    "Quest Accepted!\n\nPick up 10 trash around the barangay.\nThis teaches environmental stewardship and shared responsibility.",
                    "Bayanihan Quest");
                return;
            }

            if (activeQuest.Status == QuestStatus.Completed)
            {
                GrantReward(activeQuest.RewardMoney, activeQuest.RewardReputation);
                player.TrashCollected = 0;

                ShowGameplayPopup(
                    "Great work! You earned your reward.\n\nNext mission unlocked: Community Pantry Drive.",
                    "Quest Complete",
                    BayanihanMessageType.Success);

                activeQuest.LoadState(QuestStatus.NotStarted, 0);
                pantryMissionUnlocked = true;
                pantryMissionInProgress = true;
                pantryMissionCompleted = false;
                ClearTrashItems();
                ClearIngredientItems();
                SpawnIngredients(10);
                return;
            }

            if (activeQuest.Status == QuestStatus.InProgress)
            {
                BayanihanMessageBox.ShowQuestDialogue(
                    this,
                    "Barangay Captain",
                    "Keep going! The barangay still needs cleaning.",
                    "Bayanihan Quest");
            }
        }

        private void SwitchMap(string targetMapKey, Point spawnLocation)
        {
            if (string.Equals(currentMapKey, targetMapKey, StringComparison.Ordinal))
            {
                return;
            }

            ClearTrashItems();
            ClearIngredientItems();
            currentMapKey = targetMapKey;
            CreateMapLayout();

            playerX = spawnLocation.X;
            playerY = spawnLocation.Y;
            playerSprite.Location = spawnLocation;
            nearCaptain = false;
            nearStore = false;

            var mapQuest = GetCurrentMapQuest();
            if (mapQuest.Status == QuestStatus.InProgress)
            {
                SpawnTrash(GetTrashSpawnTarget());
            }

            if (currentMapKey == "map1" && pantryMissionInProgress && !pantryMissionCompleted)
            {
                SpawnIngredients(8);
            }
        }

        private Quest GetCurrentMapQuest()
        {
            return currentMapKey == "map2" ? map2Quest : activeQuest;
        }

        private void HandleMap2QuestInteraction()
        {
            if (map2Quest.Status == QuestStatus.NotStarted)
            {
                map2Quest.Start();
                ClearTrashItems();
                SpawnTrash(GetTrashSpawnTarget("map2"));
                BayanihanMessageBox.ShowQuestDialogue(
                    this,
                    "Road Patrol",
                    "Quest Accepted!\n\nClean the road in Map 2 by collecting 8 trash items.",
                    "Road Cleanup Quest");
                return;
            }

            if (map2Quest.Status == QuestStatus.Completed)
            {
                GrantReward(map2Quest.RewardMoney, map2Quest.RewardReputation);

                BayanihanMessageBox.Show(
                    this,
                    "Road cleanup complete! You received your reward.",
                    "Road Cleanup Complete",
                    BayanihanMessageType.Success,
                    MessageBoxButtons.OK);

                map2Quest.ResetForRepeat();
                ClearTrashItems();
                return;
            }

            BayanihanMessageBox.ShowQuestDialogue(
                this,
                "Road Patrol",
                "Keep cleaning the road. Come back when all road trash is collected.",
                "Road Cleanup Quest");
        }

        private void ClearTrashItems()
        {
            foreach (var item in trashItems)
            {
                panelMap.Controls.Remove(item.Sprite);
                item.Sprite.Dispose();
            }

            trashItems.Clear();
        }

        private void ClearIngredientItems()
        {
            foreach (var item in ingredientItems)
            {
                panelMap.Controls.Remove(item.Sprite);
                item.Sprite.Dispose();
            }

            ingredientItems.Clear();
        }

        // HUD update - nag-a-update ng lahat ng labels sa sidebar
        private void UpdateHud()
        {
            SetLabelText(lblMoney, "Money: P" + player.Money + " | Fund: P" + barangayFund + "/" + BarangayFundGoal + " | Day " + inGameDay);
            SetLabelText(lblReputation, "Reputation: " + player.Reputation + " | Rank: " + GetRankTitle() + " | Streak: " + dailyMissionStreak + " | MiniGames: " + miniGamesCompleted);
            SetLabelText(lblTrashCount, "Trash Collected: " + player.TrashCollected + " / " + player.TrashCapacity);

            var staminaWidth = (int)(panelStaminaBack.Width * (player.Stamina / 100.0));
            staminaWidth = Math.Max(0, staminaWidth);

            if (panelStaminaFill.Width != staminaWidth)
            {
                panelStaminaFill.Width = staminaWidth;
            }

            var currentQuest = GetCurrentMapQuest();
            if (currentMapKey == "map1" && pantryMissionUnlocked)
            {
                SetLabelText(lblActiveQuest, pantryMissionCompleted ? "Quest: CWTS Mission Complete" : "Quest: Community Pantry Drive");
                if (pantryMissionCompleted)
                {
                    SetLabelText(lblQuestProgress, "Mission done. You completed the barangay pantry support drive.");
                }
                else
                {
                    SetLabelText(lblQuestProgress, "Chicken: " + chickenCollected + " / " + PantryChickenGoal + " | Coconut: " + coconutCollected + " / " + PantryCoconutGoal + " | Daily: " + dailyTrashProgress + "/" + dailyTrashTarget);
                }
            }
            else if (currentQuest.Status == QuestStatus.NotStarted)
            {
                if (currentMapKey == "map2")
                {
                    SetLabelText(lblActiveQuest, "Quest: Road Cleanup (Map 2)");
                    SetLabelText(lblQuestProgress, "Press E to accept the road cleanup quest. | Daily: " + dailyTrashProgress + "/" + dailyTrashTarget);
                }
                else
                {
                    SetLabelText(lblActiveQuest, "Quest: Talk to Barangay Captain");
                    SetLabelText(lblQuestProgress, activeQuest.Description + " | Daily: " + dailyTrashProgress + "/" + dailyTrashTarget);
                }
            }
            else if (currentQuest.Status == QuestStatus.InProgress)
            {
                if (currentMapKey == "map1")
                {
                    SetLabelText(lblActiveQuest, "Quest: " + currentQuest.Title);
                    SetLabelText(lblQuestProgress, "Trash: " + currentQuest.CurrentProgress + " / " + currentQuest.ObjectiveCount + " | Daily: " + dailyTrashProgress + "/" + dailyTrashTarget);
                }
                else
                {
                    SetLabelText(lblActiveQuest, "Quest: " + currentQuest.Title);
                    SetLabelText(lblQuestProgress, "Progress: " + currentQuest.CurrentProgress + " / " + currentQuest.ObjectiveCount + " | Daily: " + dailyTrashProgress + "/" + dailyTrashTarget);
                }
            }
            else
            {
                if (currentMapKey == "map2")
                {
                    SetLabelText(lblActiveQuest, "Quest Complete!");
                    SetLabelText(lblQuestProgress, "Press E to claim your road cleanup reward. | Daily: " + dailyTrashProgress + "/" + dailyTrashTarget);
                }
                else
                {
                    SetLabelText(lblActiveQuest, "Quest Complete!");
                    SetLabelText(lblQuestProgress, "Return to Barangay Captain for rewards. | Daily: " + dailyTrashProgress + "/" + dailyTrashTarget);
                }
            }

            if (communityEventActive)
            {
                SetHintText("Event: " + communityEventName + " (" + communityEventProgress + "/" + communityEventGoal + ") until Day " + communityEventEndDay + " | P: Projects | N: NPC | M: Mini-Games");
            }
        }

        private static void SetLabelText(Label label, int value)
        {
            SetLabelText(label, value.ToString());
        }

        private static void SetLabelText(Label label, string text)
        {
            if (!string.Equals(label.Text, text, StringComparison.Ordinal))
            {
                label.Text = text;
            }
        }

        private string GetRankTitle()
        {
            if (player.Reputation >= 150)
            {
                return "Community Officer";
            }

            if (player.Reputation >= 70)
            {
                return "Team Leader";
            }

            return "Volunteer";
        }

        // Key bindings - WASD movement, E interact, P projects, N NPC, M mini-games
        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    moveUp = true;
                    ApplyPlayerSpriteImage("up");
                    break;
                case Keys.S:
                    moveDown = true;
                    ApplyPlayerSpriteImage("down");
                    break;
                case Keys.A:
                    moveLeft = true;
                    ApplyPlayerSpriteImage("left");
                    break;
                case Keys.D:
                    moveRight = true;
                    ApplyPlayerSpriteImage("right");
                    break;
                case Keys.E:
                    if (!CanInteract())
                    {
                        break;
                    }

                    if (nearStore && !isStoreModalOpen)
                    {
                        ClearMovementInput();
                        LockInteraction();
                        ShowStoreModal();
                        interactionLocked = false;
                    }
                    else if (currentMapKey == "map2")
                    {
                        ClearMovementInput();
                        LockInteraction();
                        HandleMap2QuestInteraction();
                        interactionLocked = false;
                    }
                    else if (nearCaptain)
                    {
                        ClearMovementInput();
                        LockInteraction();
                        HandleCaptainInteraction();
                        interactionLocked = false;
                    }
                    break;
                case Keys.P:
                    if (!CanInteract())
                    {
                        break;
                    }

                    ClearMovementInput();
                    LockInteraction();
                    OpenCommunityProjectsMenu();
                    interactionLocked = false;
                    break;
                case Keys.N:
                    if (!CanInteract())
                    {
                        break;
                    }

                    ClearMovementInput();
                    LockInteraction();
                    TryOpenNpcRequest();
                    interactionLocked = false;
                    break;
                case Keys.M:
                    if (!CanInteract())
                    {
                        break;
                    }

                    ClearMovementInput();
                    LockInteraction();
                    OpenMiniGameHub();
                    interactionLocked = false;
                    break;
                case Keys.Escape:
                    Close();
                    break;
            }

            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    moveUp = false;
                    break;
                case Keys.S:
                    moveDown = false;
                    break;
                case Keys.A:
                    moveLeft = false;
                    break;
                case Keys.D:
                    moveRight = false;
                    break;
            }

            if (!moveUp && !moveDown && !moveLeft && !moveRight)
            {
                ApplyPlayerSpriteImage(playerFacing);
            }
            else if (moveUp)
            {
                ApplyPlayerSpriteImage("up");
            }
            else if (moveDown)
            {
                ApplyPlayerSpriteImage("down");
            }
            else if (moveLeft)
            {
                ApplyPlayerSpriteImage("left");
            }
            else if (moveRight)
            {
                ApplyPlayerSpriteImage("right");
            }

            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private void btnBackToMenu_Click(object sender, EventArgs e)
        {
            SaveProgress();
            Close();
        }

        private void btnSaveGame_Click(object sender, EventArgs e)
        {
            SaveProgress();
            BayanihanMessageBox.Show(this, "Progress saved.", "Save Game", BayanihanMessageType.Success, MessageBoxButtons.OK);
        }

        private void btnLoadGame_Click(object sender, EventArgs e)
        {
            ReloadSavedProgress();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SaveProgress();
            base.OnFormClosing(e);
        }

        // Store items - binibili gamit yung pera ng player
        private void btnBuyGloves_Click(object sender, EventArgs e)
        {
            if (player.HasGloves)
            {
                BayanihanMessageBox.Show(this, "You already own gloves.", "Store", BayanihanMessageType.Info, MessageBoxButtons.OK);
                return;
            }

            if (!player.TrySpend(100))
            {
                BayanihanMessageBox.Show(this, "Not enough money.", "Store", BayanihanMessageType.Warning, MessageBoxButtons.OK);
                return;
            }

            player.HasGloves = true;
            BayanihanMessageBox.Show(this, "Purchased gloves. Cleaning now uses less stamina.", "Store", BayanihanMessageType.Success, MessageBoxButtons.OK);
            UpdateHud();
        }

        private void btnBuyEnergyDrink_Click(object sender, EventArgs e)
        {
            if (!player.TrySpend(30))
            {
                BayanihanMessageBox.Show(this, "Not enough money.", "Store", BayanihanMessageType.Warning, MessageBoxButtons.OK);
                return;
            }

            player.RecoverStamina(40);
            BayanihanMessageBox.Show(this, "Energy restored.", "Store", BayanihanMessageType.Success, MessageBoxButtons.OK);
            UpdateHud();
        }

        private void btnBuyTrashBag_Click(object sender, EventArgs e)
        {
            if (!player.TrySpend(60))
            {
                BayanihanMessageBox.Show(this, "Not enough money.", "Store", BayanihanMessageType.Warning, MessageBoxButtons.OK);
                return;
            }

            player.TrashCapacity += 10;
            BayanihanMessageBox.Show(this, "Trash bag upgraded. Capacity increased by 10.", "Store", BayanihanMessageType.Success, MessageBoxButtons.OK);
            UpdateHud();
        }

        // Save/Load system - XML serialization sa AppData folder
        private bool TryLoadProgress()
        {
            GameProgressData data;
            if (!GameProgressStorage.TryLoad(out data))
            {
                return false;
            }

            ApplyProgress(data);
            return true;
        }

        // Save progress - nagse-save ng lahat ng game state sa file
        private void SaveProgress()
        {
            if (player == null || activeQuest == null || map2Quest == null)
            {
                return;
            }

            try
            {
                GameProgressStorage.Save(BuildProgressSnapshot());
            }
            catch
            {
                // Ignore save failures to avoid disrupting gameplay.
            }
        }

        private void ReloadSavedProgress()
        {
            if (!TryLoadProgress())
            {
                BayanihanMessageBox.Show(this, "No saved progress was found.", "Load Game", BayanihanMessageType.Info, MessageBoxButtons.OK);
                return;
            }

            ClearTrashItems();
            ClearIngredientItems();
            CreateMapLayout();

            var currentQuest = GetCurrentMapQuest();
            if (currentQuest.Status == QuestStatus.InProgress)
            {
                SpawnTrash(GetTrashSpawnTarget());
            }

            if (currentMapKey == "map1" && pantryMissionInProgress && !pantryMissionCompleted)
            {
                SpawnIngredients(8);
            }

            UpdateHud();
            BayanihanMessageBox.Show(this, "Saved progress loaded successfully.", "Load Game", BayanihanMessageType.Success, MessageBoxButtons.OK);
        }

        // Build yung save data - kinukuha lahat ng current game state
        private GameProgressData BuildProgressSnapshot()
        {
            return new GameProgressData
            {
                Money = player.Money,
                Reputation = player.Reputation,
                Stamina = player.Stamina,
                TrashCapacity = player.TrashCapacity,
                HasGloves = player.HasGloves,
                TrashCollected = player.TrashCollected,
                TotalTrashCollected = player.TotalTrashCollected,
                ChickenCollected = chickenCollected,
                CoconutCollected = coconutCollected,
                PantryMissionUnlocked = pantryMissionUnlocked,
                PantryMissionInProgress = pantryMissionInProgress,
                PantryMissionCompleted = pantryMissionCompleted,
                InGameDay = inGameDay,
                BarangayFund = barangayFund,
                DailyMissionStreak = dailyMissionStreak,
                DailyTrashTarget = dailyTrashTarget,
                DailyTrashProgress = dailyTrashProgress,
                DailyMissionCompleted = dailyMissionCompleted,
                StreetlightLevel = streetlightLevel,
                WasteBinLevel = wasteBinLevel,
                GardenLevel = gardenLevel,
                LastNpcRequestDay = lastNpcRequestDay,
                MiniGamesCompleted = miniGamesCompleted,
                CommunityEventActive = communityEventActive,
                CommunityEventName = communityEventName,
                CommunityEventGoal = communityEventGoal,
                CommunityEventProgress = communityEventProgress,
                CommunityEventEndDay = communityEventEndDay,
                CommunityEventsCompleted = communityEventsCompleted,
                CurrentMapKey = currentMapKey,
                PlayerX = playerSprite != null ? playerSprite.Left : (int)Math.Round(playerX),
                PlayerY = playerSprite != null ? playerSprite.Top : (int)Math.Round(playerY),
                ActiveQuest = new QuestProgressData
                {
                    Status = activeQuest.Status,
                    CurrentProgress = activeQuest.CurrentProgress
                },
                Map2Quest = new QuestProgressData
                {
                    Status = map2Quest.Status,
                    CurrentProgress = map2Quest.CurrentProgress
                }
            };
        }

        // Load yung save data - ina-apply lahat ng saved values sa game
        private void ApplyProgress(GameProgressData data)
        {
            if (data == null)
            {
                return;
            }

            currentMapKey = string.Equals(data.CurrentMapKey, "map2", StringComparison.OrdinalIgnoreCase) ? "map2" : "map1";
            loadedPlayerLocation = new Point(Math.Max(0, data.PlayerX), Math.Max(0, data.PlayerY));
            hasLoadedProgress = true;

            player.LoadState(
                data.Money,
                data.Reputation,
                data.Stamina,
                data.TrashCapacity,
                data.HasGloves,
                data.TrashCollected,
                data.TotalTrashCollected);

            chickenCollected = Math.Max(0, data.ChickenCollected);
            #pragma warning disable CS0618 // PapayaCollected is kept for backward compatibility with older save files
            coconutCollected = Math.Max(0, data.CoconutCollected > 0 ? data.CoconutCollected : data.PapayaCollected);
            #pragma warning restore CS0618
            pantryMissionUnlocked = data.PantryMissionUnlocked;
            pantryMissionInProgress = data.PantryMissionInProgress;
            pantryMissionCompleted = data.PantryMissionCompleted;
            inGameDay = Math.Max(1, data.InGameDay);
            barangayFund = Math.Max(0, Math.Min(BarangayFundGoal, data.BarangayFund));
            dailyMissionStreak = Math.Max(0, data.DailyMissionStreak);
            dailyTrashTarget = Math.Max(4, data.DailyTrashTarget <= 0 ? 8 : data.DailyTrashTarget);
            dailyTrashProgress = Math.Max(0, data.DailyTrashProgress);
            dailyMissionCompleted = data.DailyMissionCompleted;
            streetlightLevel = Math.Max(0, data.StreetlightLevel);
            wasteBinLevel = Math.Max(0, data.WasteBinLevel);
            gardenLevel = Math.Max(0, data.GardenLevel);
            lastNpcRequestDay = Math.Max(0, data.LastNpcRequestDay);
            miniGamesCompleted = Math.Max(0, data.MiniGamesCompleted);
            communityEventActive = data.CommunityEventActive;
            communityEventName = data.CommunityEventName ?? string.Empty;
            communityEventGoal = Math.Max(0, data.CommunityEventGoal);
            communityEventProgress = Math.Max(0, data.CommunityEventProgress);
            communityEventEndDay = Math.Max(0, data.CommunityEventEndDay);
            communityEventsCompleted = Math.Max(0, data.CommunityEventsCompleted);
            fundGoalAnnounced = barangayFund >= BarangayFundGoal;
            if (pantryMissionCompleted)
            {
                pantryMissionInProgress = false;
                pantryMissionUnlocked = true;
            }
            if (!communityEventActive)
            {
                communityEventName = string.Empty;
                communityEventGoal = 0;
                communityEventProgress = 0;
                communityEventEndDay = 0;
            }

            activeQuest.LoadState(
                data.ActiveQuest != null ? data.ActiveQuest.Status : QuestStatus.NotStarted,
                data.ActiveQuest != null ? data.ActiveQuest.CurrentProgress : 0);

            map2Quest.LoadState(
                data.Map2Quest != null ? data.Map2Quest.Status : QuestStatus.NotStarted,
                data.Map2Quest != null ? data.Map2Quest.CurrentProgress : 0);
        }

        private static GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
        {
            if (radius < 2 || rect.Width <= 0 || rect.Height <= 0)
            {
                return null;
            }

            var diameter = radius;

            var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        private static Image MakeSpriteBackgroundTransparent(Image source)
        {
            if (source == null)
            {
                return null;
            }

            var bitmap = new Bitmap(source);
            var key = bitmap.GetPixel(0, 0);
            const int whiteThreshold = 242;

            for (var y = 0; y < bitmap.Height; y++)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    var isNearWhite = pixel.R >= whiteThreshold && pixel.G >= whiteThreshold && pixel.B >= whiteThreshold;
                    var isNearKey = Math.Abs(pixel.R - key.R) <= 10 &&
                                    Math.Abs(pixel.G - key.G) <= 10 &&
                                    Math.Abs(pixel.B - key.B) <= 10;

                    if (isNearWhite || isNearKey)
                    {
                        bitmap.SetPixel(x, y, Color.Transparent);
                    }
                }
            }

            return bitmap;
        }

        private static Image CreateIngredientFallbackImage(string ingredientType)
        {
            var bitmap = new Bitmap(40, 40);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                var isCoconut = string.Equals(ingredientType, "Coconut", StringComparison.Ordinal);
                var fillColor = isCoconut ? Color.FromArgb(255, 139, 92, 60) : Color.FromArgb(255, 245, 158, 11);
                using (var brush = new SolidBrush(fillColor))
                using (var outline = new Pen(Color.FromArgb(255, 30, 41, 59), 2F))
                {
                    g.FillEllipse(brush, 4, 4, 32, 32);
                    g.DrawEllipse(outline, 4, 4, 32, 32);
                }

                var letter = isCoconut ? "O" : "C";
                using (var textBrush = new SolidBrush(Color.White))
                using (var font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Pixel))
                {
                    var size = g.MeasureString(letter, font);
                    var x = (40F - size.Width) / 2F;
                    var y = (40F - size.Height) / 2F;
                    g.DrawString(letter, font, textBrush, x, y);
                }
            }

            return bitmap;
        }
    }
}


