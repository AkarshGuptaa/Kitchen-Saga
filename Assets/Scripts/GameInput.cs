using System;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    
    public float movementSpeed = 50f;
    private const string PLAYER_PREFS_BINDINGS = "InputBindings";
    private Transform playerTransform;
    private KeywordRecognizer keywordRecognizer; // ✅ Voice recognition module
    private Dictionary<string, Action> voiceCommands; // ✅ Stores voice commands

    public static GameInput Instance { get; private set; }

    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;
    public event EventHandler OnBindingRebind;

    private class TeleportData
    {
        public Vector3 Position;
        public Quaternion Rotation;

        public TeleportData(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }

    
    public enum Binding
    {
        Move_Up,
        Move_Down,
        Move_Left,
        Move_Right,
        Interact,
        InteractAlternate,
        Pause
    }

    private KeyCode moveUpKey = KeyCode.W;
    private KeyCode moveDownKey = KeyCode.S;
    private KeyCode moveLeftKey = KeyCode.A;
    private KeyCode moveRightKey = KeyCode.D;
    private KeyCode interactKey = KeyCode.E;
    private KeyCode interactAlternateKey = KeyCode.Q;
    private KeyCode pauseKey = KeyCode.Escape;

    private Dictionary<KeyCode, TeleportData> teleportPositions; // Stores teleport destinations

    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one GameInput instance exists!");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player GameObject not found! Make sure your player has the 'Player' tag.");
        }

        LoadKeyBindings();
        InitializeTeleportPositions();
        InitializeVoiceRecognition(); // Setup voice commands
    }

    private void Update()
    {
        if (Input.GetKeyDown(interactKey))
            OnInteractAction?.Invoke(this, EventArgs.Empty);

        if (Input.GetKeyDown(interactAlternateKey))
            OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);

        if (Input.GetKeyDown(pauseKey))
            OnPauseAction?.Invoke(this, EventArgs.Empty);

        HandleTeleportation(); // Check for teleport inputs
    }

    public Vector2 GetMovementVectorNormalized()
    {
        float x = 0f;
        float y = 0f;

        if (Input.GetKey(moveUpKey)) y += 1f;
        if (Input.GetKey(moveDownKey)) y -= 1f;
        if (Input.GetKey(moveLeftKey)) x -= 1f;
        if (Input.GetKey(moveRightKey)) x += 1f;

        return new Vector2(x, y).normalized;
    }

    public string GetBindingText(Binding binding)
    {
        switch (binding)
        {
            case Binding.Move_Up: return moveUpKey.ToString();
            case Binding.Move_Down: return moveDownKey.ToString();
            case Binding.Move_Left: return moveLeftKey.ToString();
            case Binding.Move_Right: return moveRightKey.ToString();
            case Binding.Interact: return interactKey.ToString();
            case Binding.InteractAlternate: return interactAlternateKey.ToString();
            case Binding.Pause: return pauseKey.ToString();
            default: return "Unknown";
        }
    }

    private void LoadKeyBindings()
    {
        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
        {
            string[] keys = PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS).Split(',');
            if (keys.Length == 7)
            {
                moveUpKey = (KeyCode)Enum.Parse(typeof(KeyCode), keys[0]);
                moveDownKey = (KeyCode)Enum.Parse(typeof(KeyCode), keys[1]);
                moveLeftKey = (KeyCode)Enum.Parse(typeof(KeyCode), keys[2]);
                moveRightKey = (KeyCode)Enum.Parse(typeof(KeyCode), keys[3]);
                interactKey = (KeyCode)Enum.Parse(typeof(KeyCode), keys[4]);
                interactAlternateKey = (KeyCode)Enum.Parse(typeof(KeyCode), keys[5]);
                pauseKey = (KeyCode)Enum.Parse(typeof(KeyCode), keys[6]);
            }
        }
    }

    
    private void InitializeTeleportPositions()
    {
        teleportPositions = new Dictionary<KeyCode, TeleportData>
        {
            { KeyCode.H, new TeleportData(new Vector3(5.978f, 0f, 1.5f), Quaternion.Euler(0f, 0f, 0f)) }, // Ham
            { KeyCode.P, new TeleportData(new Vector3(3.03f, 0f, -3.02f), Quaternion.Euler(0f, 0f, 0f)) }, // Plate
            { KeyCode.C, new TeleportData(new Vector3(0f, 0f, 1.5f), Quaternion.Euler(0f, 0f, 0f)) }, // Cheese
            { KeyCode.B, new TeleportData(new Vector3(-3.83f, 0f, 1.5f), Quaternion.Euler(0f, 0f, 0f)) }, // Bun
            { KeyCode.L, new TeleportData(new Vector3(-1.45f, 0f, -3.02f), Quaternion.Euler(0f, 0f, 0f)) }, // Lettuce
            { KeyCode.T, new TeleportData(new Vector3(0.05f, 0f, -3.02f), Quaternion.Euler(0f, 0f, 0f)) }, // Tomato
            { KeyCode.G, new TeleportData(new Vector3(4.32f, 0f, 1.5f), Quaternion.Euler(0f, 0f, 0f)) }, // Gas
            { KeyCode.X, new TeleportData(new Vector3(-4.43f, 0f, -3.02f), Quaternion.Euler(0f, 0f, 0f)) }, // Trash
            { KeyCode.R, new TeleportData(new Vector3(4.52f, 0f, -3.02f), Quaternion.Euler(0f, 0f, 0f)) }, // Serve
            { KeyCode.F, new TeleportData(new Vector3(-0.94f, 0f, 2.3f), Quaternion.Euler(0f, 0f, 0f)) }, // Empty
            { KeyCode.J, new TeleportData(new Vector3(3.03f, 0f, 1.5f), Quaternion.Euler(0f, 0f, 0f)) }, // Board
        };
    }

    private void HandleTeleportation()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform is NULL! Ensure the Player is tagged as 'Player'.");
            return;
        }

        foreach (var key in teleportPositions.Keys)
        {
            if (Input.GetKeyDown(key))
            {
                TeleportData teleportData = teleportPositions[key];
                Debug.Log($"Teleporting to {key} at {teleportData.Position}");

                Rigidbody rb = playerTransform.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.MovePosition(teleportData.Position);
                    rb.isKinematic = false;
                }
                else
                {
                    playerTransform.position = teleportData.Position;
                }

                // Apply Rotation
                playerTransform.localRotation = teleportData.Rotation;
            }
        }
    }

    // Voice Recognition Setup
    private void InitializeVoiceRecognition()
    {
        voiceCommands = new Dictionary<string, Action>
        {
            { "start", () => HandleVoiceCommand(KeyCode.E) },
            { "place", () => HandleVoiceCommand(KeyCode.E) },
            { "cook", () => HandleVoiceCommand(KeyCode.E) },
            { "pick", () => HandleVoiceCommand(KeyCode.E) },
            { "gas", () => HandleVoiceCommand(KeyCode.G) },
            { "chicken", () => HandleVoiceCommand(KeyCode.H) },
            { "cheese", () => HandleVoiceCommand(KeyCode.C) },
            { "bun", () => HandleVoiceCommand(KeyCode.B) },
            { "lettuce", () => HandleVoiceCommand(KeyCode.L) },
            { "tomato", () => HandleVoiceCommand(KeyCode.T) },
            { "plate", () => HandleVoiceCommand(KeyCode.P) },
            { "send", () => HandleVoiceCommand(KeyCode.R) },
            { "bin", () => HandleVoiceCommand(KeyCode.X) },
            { "chop", () => HandleVoiceCommand(KeyCode.Q) },
            { "counter", () => HandleVoiceCommand(KeyCode.F) },
            { "empty", () => HandleVoiceCommand(KeyCode.F) },
            { "board", () => HandleVoiceCommand(KeyCode.J) },
        };

        keywordRecognizer = new KeywordRecognizer(voiceCommands.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keywordRecognizer.Start();

        Debug.Log("Voice Recognition Started!");
    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log($"Voice Command Recognized: {speech.text}");
        if (voiceCommands.ContainsKey(speech.text))
        {
            voiceCommands[speech.text].Invoke();
        }
    }
    
    
    private void HandleVoiceCommand(KeyCode key)
    {
        Debug.Log($"Simulating key press: {key}");
        
        switch (key)
        {
            case KeyCode.E:
                OnInteractAction?.Invoke(this, EventArgs.Empty);
                break;

            case KeyCode.Q:
                OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
                break;

            case KeyCode.G:
            case KeyCode.X:
            case KeyCode.T:
            case KeyCode.H:
            case KeyCode.J:
            case KeyCode.F:
            case KeyCode.R:
            case KeyCode.P:
            case KeyCode.C:
            case KeyCode.B:
            case KeyCode.L:
                if (teleportPositions.ContainsKey(key))
                {
                    TeleportData teleportData = teleportPositions[key];
                    Debug.Log($"Teleporting via voice to {teleportData.Position}");

                    Rigidbody rb = playerTransform.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.isKinematic = true;
                        rb.MovePosition(teleportData.Position);
                        rb.isKinematic = false;
                    }
                    else
                    {
                        playerTransform.position = teleportData.Position;
                    }

                    // Apply rotation too
                    playerTransform.localRotation = teleportData.Rotation;
                }
                break;
        }
    }


    private void SaveKeyBindings()
    {
        string bindings = $"{moveUpKey},{moveDownKey},{moveLeftKey},{moveRightKey},{interactKey},{interactAlternateKey},{pauseKey}";
        PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, bindings);
        PlayerPrefs.Save();
        Debug.Log("Keybindings Saved: " + bindings);
    }

    public void RebindBinding(Binding binding, KeyCode newKey)
    {
        switch (binding)
        {
            case Binding.Move_Up: moveUpKey = newKey; break;
            case Binding.Move_Down: moveDownKey = newKey; break;
            case Binding.Move_Left: moveLeftKey = newKey; break;
            case Binding.Move_Right: moveRightKey = newKey; break;
            case Binding.Interact: interactKey = newKey; break;
            case Binding.InteractAlternate: interactAlternateKey = newKey; break;
            case Binding.Pause: pauseKey = newKey; break;
        }

        SaveKeyBindings();
        OnBindingRebind?.Invoke(this, EventArgs.Empty);
    }

}