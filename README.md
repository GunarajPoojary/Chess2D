# ♟️ Unity Chess 2D

A fully functional **2D Chess game built with Unity**.\
Features a minimax-based AI opponent, interactive UI, highlights,
captured piece tracking, timers, and more.

------------------------------------------------------------------------

## 🎮 Features

-   **Classic Chess Mechanics**\
    All standard chess rules are supported (movement, captures, win
    conditions).

-   **AI Opponent**

    -   Minimax algorithm with alpha-beta pruning.\
    -   Adjustable difficulty depth (via settings).

-   **Player Controls**

    -   Mouse and touch input support.\
    -   Tile highlights for moves and captures.\
    -   Turn-based system (Player ↔ AI).

-   **User Interface**

    -   Main Menu with difficulty and team color selection.\
    -   Move history with chess notation.\
    -   Captured pieces panel.\
    -   Turn indicator and timers for both players.\
    -   Win stats screen.

-   **Audio & Visual Feedback**

    -   Customizable move, capture, check, and checkmate sounds.\
    -   Highlight effects for selection, empty tiles, and captures.\
    -   UI animations and sound effects using DOTween.

-   **Tutorial Mode**

    -   Step-by-step tutorial to guide new players.

-   **Persistence**

    -   Player preferences (difficulty, colors, audio settings) are
        saved.\
    -   Cross-platform support (PC, WebGL, Mobile).

------------------------------------------------------------------------

## 🛠️ Tech Stack

-   **Engine**: Unity\
-   **Language**: C#\
-   **Design Patterns**:
    -   Event-driven architecture (`GameEvents`)\
    -   Factory pattern for chess piece instantiation\
    -   Strategy pattern for movement logic

------------------------------------------------------------------------

## 🚀 Getting Started

### 1. Clone Repository

``` bash
git clone https://github.com/your-username/UnityChess2D.git
cd UnityChess2D
```

### 2. Open in Unity

-   Unity version: **2021.3 LTS** or later (recommended).\
-   Open the project in Unity Hub.

### 3. Play

-   Open the **MainMenu scene**.\
-   Press ▶️ Play in Unity Editor.

------------------------------------------------------------------------

## 📂 Project Structure

    Assets/
     ├── Scripts/
     │   ├── AI/               # Minimax AI logic
     │   ├── Audio/            # Audio configs & manager
     │   ├── Board/            # Chess board and utilities
     │   ├── ChessPieces/      # Piece data, factories, movement strategies
     │   ├── Events/           # Event channels for decoupled systems
     │   ├── Highlighter/      # Tile highlight logic
     │   ├── Player/           # Player input controller
     │   ├── UI/               # UI system (menus, timers, captured pieces, etc.)
     │   ├── GameManager.cs    # Main game flow
     │   ├── Timer.cs          # Game timers
     │   └── TurnManager.cs    # Turn handling
     └── ScriptableObjects/    # Piece databases, highlight sprites

------------------------------------------------------------------------

## ⚙️ Key Components

-   **`AIController.cs`** → Minimax-based chess AI.\
-   **`PlayerController.cs`** → Handles user input & move execution.\
-   **`ChessBoard.cs`** → Core board representation and piece
    placement.\
-   **`MoveStrategyFactory.cs`** → Provides move strategies per piece
    type.\
-   **`UIManager.cs`** → Handles UI initialization & updates.\
-   **`GameManager.cs`** → Orchestrates game initialization, board
    setup, win conditions.

------------------------------------------------------------------------

## 🎨 Screenshots

*(Add screenshots or GIFs of your game here!)*

------------------------------------------------------------------------

## 📌 Future Improvements

-   Online multiplayer support\
-   Undo/redo moves\
-   Advanced AI with opening book / endgame tablebase\
-   Animations for piece movement

------------------------------------------------------------------------

## 📄 License

This project is licensed under the **MIT License**.\
You are free to use, modify, and distribute with attribution.
