using Il2CppMashBox.BMX_Physics_Development;
using Il2CppMashBox.Addons.Prototyping;
using Il2CppMashBox.Addons.ContentManagment;
using MelonLoader;
using UnityEngine;
using Il2CppCinemachine;

[assembly: MelonInfo(typeof(MashBoxMod.Main), "MashBoxMod", "0.0.2", "nolew, xrowe")]
[assembly: MelonGame("Mash Games", "Mash Box")]

namespace MashBoxMod
{
    public class Main : MelonMod
    {
        // Menu variables
        bool menuOpen = false;
        int currentTab = 0;
        int currentTrickInput = 0;
        private Rect windowRect;

        // Physics variables
        bool spinAssist = true;
        bool flightAugment = true;
        bool breakForceBool = false;
        bool enableSpokes = false;
        bool disableLevelInAir = false;
        float droneMass = 10f;
        float gravity = 12.5f;
        float pumpForce = 0.2f;
        float spinTorque = 1f;
        float pedalForce = 5f;
        float maxSpeed = 9f;
        float camLerp = 6f;
        float steerDamp = 5f;
        float fovValue = 60f;
        float breakForce = 9999999999f;
        string breakForceString = "";

        // Misc variables
        bool bShowInstructionCanvas = true;
        bool bHapticFeedBack = true;

        // References
        GameObject? ProtoSports;
        GameObject? InstructionCanvas;
        GameObject? hapticFeedBack;
        GameObject? AccelObj;
        GameObject? drone;
        GameObject? BMXChassis;
        BMXCMCameraTarget? camTarget;
        CinemachineVirtualCamera? virtualCam;
        LensSettings lensSettings;
        VehicleController? bmxVehicleController;
        FlightAugmentTest? bmxFlightAugment;
        VehicleBalancePID? bmxVehicleBalance;
        FlightPrediction? bmxFlightPrediction;
        BMXCollisionHandler? bmxCollisionHandler;
        Event? e;


        // Tricks
        Rebind? tricks;

        public static string PSportsPrefix = "Proto Sports";

        // Categories dictionary
        private Dictionary<string, (string displayName, List<EquipSlotNew> slots)> categories = new Dictionary<string, (string displayName, List<EquipSlotNew> slots)>
        {
            { "BB", ("Bottom Bracket", new List<EquipSlotNew>()) },
            { "Bars", ("Handlebars", new List<EquipSlotNew>()) },
            { "Bar End", ("Bar Ends", new List<EquipSlotNew>()) },
            { "Guard", ("Guards", new List<EquipSlotNew>()) },
            { "Hub", ("Hubs", new List<EquipSlotNew>()) },
            { "BMX_Nipples", ("BMX Nipples", new List<EquipSlotNew>()) },
            { "BMX_Rim", ("BMX Rim", new List<EquipSlotNew>()) },
            { "BMX_Spokes", ("BMX Spokes", new List<EquipSlotNew>()) },
            { "BMX_Tire", ("BMX Tire", new List<EquipSlotNew>()) },
            { "Chain", ("Chain", new List<EquipSlotNew>()) },
            { "Crank Arm", ("Crank Arm", new List<EquipSlotNew>()) },
            { "Forks", ("Forks", new List<EquipSlotNew>()) },
            { "Frame", ("Frame", new List<EquipSlotNew>()) },
            { "Grip", ("Grips", new List<EquipSlotNew>()) },
            { "Headset", ("Headsets", new List<EquipSlotNew>()) },
            { "Pedal", ("Pedals", new List<EquipSlotNew>()) },
            { "BMX_Peg_", ("BMX Pegs", new List<EquipSlotNew>()) },
            { "Seat Post", ("Seat Posts", new List<EquipSlotNew>()) },
            { "Seat", ("Seats", new List<EquipSlotNew>()) },
            { "Sprocket", ("Sprockets", new List<EquipSlotNew>()) },
            { "Stem Cap", ("Stem Caps", new List<EquipSlotNew>()) },
            { "StemBolt", ("Stem Bolts", new List<EquipSlotNew>()) },
            { "Stem", ("Stems", new List<EquipSlotNew>()) },
        };

        // Indices for cycling through items in categories
        private Dictionary<string, int> categoryArticleIndices = new Dictionary<string, int>();


        // For scroll view in GUI
        private Vector2 scrollPosition = Vector2.zero;

        void UpdateObjects()
        {
            // Find ProtoSports
            if (GameObject.Find("Proto Sports"))
            {
                PSportsPrefix = "Proto Sports(Clone)";
                ProtoSports = GameObject.Find(PSportsPrefix);
            }
            else if (GameObject.Find("Proto Sports(Clone)"))
            {
                PSportsPrefix = "Proto Sports";
                ProtoSports = GameObject.Find(PSportsPrefix);
            }
            else
            {
                LoggerInstance.Warning("Proto Sports GameObject not found.");
            }

            // Find InstructionCanvas
            InstructionCanvas = GameObject.Find($"{PSportsPrefix}/Instructions Canvas");
            if (InstructionCanvas == null)
            {
                LoggerInstance.Warning("InstructionCanvas GameObject not found.");
            }

            // Find HapticFeedback
            hapticFeedBack = GameObject.Find("Haptic Feedback Manager(Clone)") ?? GameObject.Find("Haptic Feedback Manager");
            if (hapticFeedBack == null)
            {
                LoggerInstance.Warning("Haptic Feedback Manager GameObject not found.");
            }

            // Find Drone
            drone = GameObject.Find("Proto_Drone");
            if (drone != null)
            {
                var droneRigidbody = drone.GetComponent<Rigidbody>();
                if (droneRigidbody != null)
                {
                    droneRigidbody.mass = droneMass;
                }
                else
                {
                    LoggerInstance.Warning("Rigidbody component not found on Proto_Drone.");
                }
            }
            else
            {
                LoggerInstance.Warning("Proto_Drone GameObject not found.");
            }

            // Find Acceleration Force Object
            AccelObj = GameObject.Find("Acceleration Force");
            if (AccelObj != null)
            {
                var accelerationForce = AccelObj.GetComponent<SimpleAccelerationForce>();
                if (accelerationForce != null)
                {
                    accelerationForce._force = pedalForce;
                    accelerationForce._topSpeed = maxSpeed;
                }
                else
                {
                    LoggerInstance.Warning("SimpleAccelerationForce component not found on Acceleration Force GameObject.");
                }
            }
            else
            {
                LoggerInstance.Warning("Acceleration Force GameObject not found.");
            }

            // Find BMXChassis
            BMXChassis = GameObject.Find("Chassis Body");
            if (BMXChassis != null)
            {
                var spinSystem = BMXChassis.GetComponent<SpinSystem>();
                if (spinSystem != null)
                {
                    spinSystem._torqueMult = spinTorque;
                }
                else
                {
                    LoggerInstance.Warning("SpinSystem component not found on Chassis Body.");
                }

                var vehicleController = BMXChassis.GetComponent<VehicleController>();
                if (vehicleController != null)
                {
                    vehicleController._airSpinAssist = spinAssist;
                }
                else
                {
                    LoggerInstance.Warning("VehicleController component not found on Chassis Body.");
                }

                var pumpForce = BMXChassis.GetComponent<PumpSystem>();
                if (pumpForce != null)
                {
                    pumpForce._pumpForce = this.pumpForce;
                }
                else
                {
                    LoggerInstance.Warning("SimplePumpForce component not found on Chassis Body.");
                }
            }
            else
            {
                LoggerInstance.Warning("Chassis Body GameObject not found.");
            }

            // Find Camera Target
            var camTargetObject = GameObject.Find("BMX Camera Target");
            if (camTargetObject != null)
            {
                camTarget = camTargetObject.GetComponent<BMXCMCameraTarget>();
                if (camTarget != null)
                {
                    camTarget._rotLerp = camLerp;
                }
                else
                {
                    LoggerInstance.Warning("BMXCMCameraTarget component not found on BMX Camera Target.");
                }
            }
            else
            {
                LoggerInstance.Warning("BMX Camera Target GameObject not found.");
            }

            // Find Virtual Camera
            var virtualCamObject = GameObject.Find("2d Virtual Camera");
            if (virtualCamObject != null)
            {
                virtualCam = virtualCamObject.GetComponent<CinemachineVirtualCamera>();
                if (virtualCam != null)
                {
                    lensSettings = virtualCam.m_Lens;
                    lensSettings.FieldOfView = fovValue;
                    virtualCam.m_Lens = lensSettings;
                }
                else
                {
                    LoggerInstance.Warning("CinemachineVirtualCamera component not found on 2d Virtual Camera.");
                }
            }
            else
            {
                LoggerInstance.Warning("2d Virtual Camera GameObject not found.");
            }

            // Get VehicleController from BMXChassis
            if (BMXChassis != null)
            {
                bmxVehicleController = BMXChassis.GetComponentInChildren<VehicleController>();
                if (bmxVehicleController != null)
                {
                    bmxVehicleController._steerDampRate = steerDamp;
                }
                else
                {
                    LoggerInstance.Warning("VehicleController component not found in children of Chassis Body.");
                }

                // Get FlightAugmentTest
                bmxFlightAugment = BMXChassis.GetComponentInChildren<FlightAugmentTest>();
                if (bmxFlightAugment != null)
                {
                    bmxFlightAugment.enabled = flightAugment;
                }
                else
                {
                    LoggerInstance.Warning("FlightAugmentTest component not found in children of Chassis Body.");
                }

                // Get CollisionHandler
                bmxCollisionHandler = BMXChassis.GetComponent<BMXCollisionHandler>();
                if (bmxCollisionHandler == null)
                {
                    LoggerInstance.Warning("BMXCollisionHandler component not found on Chassis Body.");
                }

                // Get VehicleBalancePID
                bmxVehicleBalance = BMXChassis.GetComponent<VehicleBalancePID>();
                if (bmxVehicleBalance == null)
                {
                    LoggerInstance.Warning("VehicleBalancePID component not found on Chassis Body.");
                }

                // Get FlightPrediction
                bmxFlightPrediction = BMXChassis.GetComponent<FlightPrediction>();
                if (bmxFlightPrediction == null)
                {
                    LoggerInstance.Warning("FlightPrediction component not found on Chassis Body.");
                }

                // Update joints' break force
                var joints = BMXChassis.GetComponentsInChildren<Joint>();
                foreach (var joint in joints)
                {
                    if (joint != null)
                    {
                        joint.breakForce = breakForce;
                        breakForceString = breakForce.ToString();
                    }
                    else
                    {
                        LoggerInstance.Warning("Encountered a null Joint in children of Chassis Body.");
                    }
                }
            }
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            UpdateObjects();
        }

        //an attempt was made to fix flairs, side quest abandoned for now
        //public override void OnFixedUpdate()
        //{
        //    if (BMXChassis == null)
        //        return;

        //    bmxCollisionHandler = BMXChassis.GetComponent<BMXCollisionHandler>();
        //    bmxVehicleBalance = BMXChassis.GetComponent<VehicleBalancePID>();
        //    bmxFlightPrediction = BMXChassis.GetComponent<FlightPrediction>();

        //    if (disableLevelInAir == true && bmxFlightAugment != null && bmxVehicleBalance != null && bmxFlightPrediction != null)
        //    {
        //        if (bmxFlightAugment._enteredFlight)
        //        {
        //            bmxVehicleBalance._runPID = false;
        //        }
        //        if (bmxFlightPrediction._timeLeftInAir <= 0.65)
        //        {
        //            bmxVehicleBalance._runPID = true;
        //        }
        //    }
        //}

        void WndProc(int windowID)
        {
            GUI.backgroundColor = Color.green;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<b>Physics</b>"))
            {
                currentTab = 0;
            }
            if (GUILayout.Button("<b>Tricks</b>"))
            {
                currentTab = 1;
            }
            if (GUILayout.Button("<b>Character</b>"))
            {
                currentTab = 2;
            }
            if (GUILayout.Button("<b>Misc</b>"))
            {
                currentTab = 3;
            }
            if (GUILayout.Button("<b>Bike</b>"))
            {
                currentTab = 4;
            }
            GUILayout.EndHorizontal();

            // Menu
            switch(currentTab)
            {
                // Physics Tab
                case 0:
                    spinAssist = GUILayout.Toggle(spinAssist, "<b>Spin Assist</b>");

                    flightAugment = GUILayout.Toggle(flightAugment, "<b>Flight Augment</b>");

                    //disableLevelInAir = GUILayout.Toggle(disableLevelInAir, "<b>Disable Leveling in air</b>");

                    GUILayout.Label($"Gravity = {gravity}");
                    gravity = GUILayout.HorizontalSlider(gravity, 0, 30f);
                    Physics.gravity = new Vector3(0f, -this.gravity, 0f);

                    GUILayout.Label($"Pump Force = {pumpForce}");
                    pumpForce = GUILayout.HorizontalSlider(pumpForce, 0, 5f);

                    GUILayout.Label($"Spin Speed Multi = {spinTorque}");
                    spinTorque = GUILayout.HorizontalSlider(spinTorque, 0, 10f);

                    GUILayout.Label($"Pedal Force = {pedalForce}");
                    pedalForce = GUILayout.HorizontalSlider(pedalForce, 0, 25f);

                    GUILayout.Label($"Max Speed = {maxSpeed}");
                    maxSpeed = GUILayout.HorizontalSlider(maxSpeed, 0, 25f);

                    GUILayout.Label($"Drone Mass = {droneMass}");
                    droneMass = GUILayout.HorizontalSlider(droneMass, 10f, 9999f);

                    GUILayout.Label($"Camera Rotation Lerp = {camLerp}");
                    camLerp = GUILayout.HorizontalSlider(camLerp, 0f, 5f);

                    GUILayout.Label($"Camera Field of View = {fovValue}");
                    fovValue = GUILayout.HorizontalSlider(fovValue, 1f, 200f);

                    GUILayout.Label($"Steering Dampening = {steerDamp}");
                    steerDamp = GUILayout.HorizontalSlider(steerDamp, 1f, 5f);

                    breakForceBool = GUILayout.Toggle(breakForceBool, "<b>Enable Break Force</b>");
                    GUILayout.Label($"Rigidbody Joint Break Force = {breakForce}");
                    breakForceString = GUILayout.TextField(breakForceString);

                    break; 

                // Tricks Tab
                case 1:
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Left Bumper"))
                    {
                        currentTrickInput = 0;
                    }
                    if (GUILayout.Button("Right Bumper"))
                    {
                        currentTrickInput = 1;
                        LoggerInstance.Msg("Swapped right bumper input");
                        // Example of swapping tricks (uncomment and adjust as needed)
                        // tricks.BindTrick(TrickInput.RightBumper, Direction.Up, tricks.Cannonball);
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Left Trigger"))
                    {
                        currentTrickInput = 2;
                        GUI.backgroundColor = Color.red;
                    }
                    if (GUILayout.Button("Right Trigger"))
                    {
                        currentTrickInput = 3;
                        GUI.backgroundColor = Color.red;
                    }
                    GUILayout.EndHorizontal();

                    if (GUILayout.Button("Both Bumpers"))
                    {
                        currentTrickInput = 4;
                    }
                    if (GUILayout.Button("Both Triggers"))
                    {
                        currentTrickInput = 5;
                    }

                    // TODO: Make UI for binding tricks
                    switch (currentTrickInput)
                    {
                        case 0:
                            GUILayout.Label("Left Bumper!");
                            break;
                        case 1:
                            GUILayout.Label("Right Bumper!");
                            break;
                        case 2:
                            GUILayout.Label("Left Trigger!");
                            break;
                        case 3:
                            GUILayout.Label("Right Trigger!");
                            break;
                        case 4:
                            GUILayout.Label("Both Bumpers!");
                            break;
                        case 5:
                            GUILayout.Label("Both Triggers!");
                            break;
                    }

                    break;

                // Character Tab
                case 2: 
                    // TODO
                    break;

                // Misc Tab
                case 3:
                    bShowInstructionCanvas = GUILayout.Toggle(bShowInstructionCanvas, "<b>Instruction Canvas</b>");
                    if (InstructionCanvas != null)
                    {
                        InstructionCanvas.SetActive(bShowInstructionCanvas);
                    }

                    bHapticFeedBack = GUILayout.Toggle(bHapticFeedBack, "<b>Vibration</b>");
                    if (hapticFeedBack != null)
                    {
                        hapticFeedBack.SetActive(bHapticFeedBack);
                    }

                    GUI.backgroundColor = Color.green;
                    if (GUILayout.Button("<b>Save Settings</b>"))
                    {
                        LoggerInstance.Msg("Saved Settings");
                    }
                    break;

                // Bike Tab
                case 4:
                    // Clear previous entries in categories
                    foreach (var key in new List<string>(categories.Keys))
                    {
                        categories[key] = (categories[key].displayName, new List<EquipSlotNew>());
                    }

                    // Find all instances of EquipSlotNew in the scene
                    var equipSlots = GameObject.FindObjectsOfType<EquipSlotNew>();

                    // Check if any EquipSlotNew components were found
                    if (equipSlots == null || equipSlots.Length == 0)
                    {
                        GUILayout.Label("No EquipSlotNew components found in the scene.");
                    }
                    else
                    {
                        // Iterate over each EquipSlotNew component
                        foreach (var equipSlot in equipSlots)
                        {
                            // Ensure the equipSlot and its GameObject are not null
                            if (equipSlot != null && equipSlot.gameObject != null)
                            {
                                string gameObjectName = equipSlot.gameObject.name;

                                // Determine which category the equipSlot belongs to
                                bool foundCategory = false;
                                foreach (var categoryKey in categories.Keys)
                                {
                                    if (gameObjectName.Contains(categoryKey))
                                    {
                                        // Add equipSlot to the category's list
                                        categories[categoryKey].slots.Add(equipSlot);
                                        foundCategory = true;
                                        break;
                                    }
                                }

                                if (!foundCategory)
                                {
                                    LoggerInstance.Warning($"EquipSlotNew component on {gameObjectName} does not match any category.");
                                }
                            }
                            else
                            {
                                LoggerInstance.Warning("Encountered a null EquipSlotNew component or GameObject.");
                            }
                        }

                        // Create buttons for each category
                        foreach (var category in categories)
                        {
                            var categoryKey = category.Key;
                            var displayName = category.Value.displayName;
                            var slots = category.Value.slots;

                            if (slots.Count > 0)
                            {
                                // Initialize the article index for the category if not already done
                                if (!categoryArticleIndices.ContainsKey(categoryKey))
                                {
                                    categoryArticleIndices[categoryKey] = 0;
                                }

                                GUILayout.BeginHorizontal();

                                // Create 'Previous' button
                                if (GUILayout.Button($"< Prev {displayName}"))
                                {
                                    // For each EquipSlotNew in the category, adjust currentArticle and call ChangeArticle
                                    foreach (var equipSlot in slots)
                                    {
                                        if (equipSlot != null && equipSlot._avaliableAssets != null)
                                        {
                                            int totalArticles = equipSlot._avaliableAssets.Count;

                                            // Adjust currentArticle
                                            equipSlot.currentArticle -= 2;

                                            // Handle wrapping
                                            if (equipSlot.currentArticle < 0)
                                            {
                                                equipSlot.currentArticle += totalArticles;
                                                if (equipSlot.currentArticle < 0)
                                                {
                                                    equipSlot.currentArticle += totalArticles;
                                                }
                                            }

                                            // Call ChangeArticle() which will increment currentArticle by 1
                                            equipSlot.ChangeArticle();
                                        }
                                        else
                                        {
                                            LoggerInstance.Warning($"EquipSlotNew in category {displayName} is null or has no available assets.");
                                        }
                                    }
                                }

                                // Create 'Next' button
                                if (GUILayout.Button($"Next {displayName} >"))
                                {
                                    // For each EquipSlotNew in the category, call ChangeArticle()
                                    foreach (var equipSlot in slots)
                                    {
                                        if (equipSlot != null && equipSlot._avaliableAssets != null)
                                        {
                                            // Call ChangeArticle() which increments currentArticle by 1
                                            equipSlot.ChangeArticle();
                                        }
                                        else
                                        {
                                            LoggerInstance.Warning($"EquipSlotNew in category {displayName} is null or has no available assets.");
                                        }
                                    }
                                }

                                GUILayout.EndHorizontal();
                            }
                        }
                    }
                    break;
            }
        }



        void FixManuals()
        {
            //pls mash i dont wanna make an AnimationCurve
        }

        public override void OnGUI()
        {
            // Menu toggle
            e = Event.current;
            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.N)
                {
                    menuOpen = !menuOpen;

                    if (menuOpen)
                    {
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                    }
                    else
                    {
                        Cursor.visible = false;
                        Cursor.lockState = CursorLockMode.Locked;
                    }
                }

                if (e.keyCode == KeyCode.O)
                {
                    UpdateObjects();
                }
            }
            
            if (menuOpen)
            {
                GUI.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
                windowRect = GUI.Window(0, new Rect(Screen.width / 2f - (800f / 2), 250f, 800f, 600f), new Action<int>(this.WndProc), "<b>MashBoxMod</b>");
            }
        }
    }
}