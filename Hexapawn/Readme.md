HEXAPAWN - AI self learning app.

This application is my first coding project before taking any official education in programming.

Game rules:
1. Game items:
	- Board Game. 3x3 chess board.
	- 6 chess pawns. 3 white, 3 black.
2. Players:
	- White: Human player.
	- Black: Computer player.
3. Pawn movement and placement rules:
	- White player always moves first.
	- White pawns are placed in '1' row.
	- Black pawns are placed in '3' row.
	- Pawn can move one cell forward if forward cell is free.
	- Pawn can 'kill' opponent pawn if it is forward-diagonaly (like in chess).
	- Pawn can't go back.
	- Player wins if at least one pawn reaches enemy starting row or after player movement opponents has no movements left.

Functionality:
	- To begin a game, press 'Start New Game' button.
	- Game pieces are placed on a board and game begins.
	- Human player on its turn selects one pawn and moves where possible. To cancel selection click on selected pawn again.
		- After move is made computer checks for victory conditions. If human player won, then AI player substract 1 
		last move probability point from probabiblity points.
		- If human player didn't won, then end player turn and begin AI player turn.
	- AI player checks fake database.
		- There is no data for current situation in database:
			- Create situation data table.
			- Assign default probability points for all possible moves (1).
		- There is data for current situation in database:
			- Randomly pick one possible move.
				- Each move has probability points. This determines how often this move will be picked. Higher value means that this 
					will be picked more often than those with lower value.
			- Do selected move.
		- Check for victory conditions.
			- If victory. Add 1 probability point to last move probability points.
			- If victory conditions not met. End turn and begin human turn.

Basicaly AI player gathers data of each game and learns to pick good moves and AI wins more and more and finaly it always wins.
