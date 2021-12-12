Instructions:
- Create a new World object by right clicking in the assets, navigating to Create -> Scriptable Objects -> World
- Open the World Node Editor by double clicking / opening the world you want to edit

1. EDITOR FEATURES
	A. RIGHT CLICK FUNCTIONALITY
		- Right click on empty areas to create new nodes
		- Right click on a node to create new connections and destroy nodes
		- Right click on connectors to destroy them
		- Right click while holding an unset connector to cancel it or:
			- If over an empty space: create a new node
			- If over a node: set the destination of the connector to the selected node
				- Attempting to set the destination to the starting node results in a warning saying that you can't make the entrance and destination the same node, and allows you to select a different node


	B. LEFT CLICK FUNCTIONALITY
		- Left click on a node to select it, which opens it in the inspector
		- Left click on a connector to select it, which opens it in the inspector
		- Left click while holding an unset connector over a node to set the destination of the connector to the selected node
			- Attempting to set the destination to the starting node results in a warning saying that you can't make the entrance and destination the same node, and allows you to select a different node
			- Left clicking on an empty space results in nothing


	C. DRAGGING FUNCTIONALITY
		- Left clicking and dragging on a node results in that node being moved along with the mouse
		- Left clicking and dragging on an empty space (or a connector, unfortunately) results in panning the entire viewport along with the mouse
		- Middle Mouse clicking and dragging results in panning the entire viewport along with the mouse


	D. WISHLIST
		- Features that I wanted to add but was unable to or did not have time for
			- Zoom: Scaling the window so that more or less nodes could be viewed at a time
			- Multi-select: Allow mass deleting and mass dragging of nodes


2. OBJECT COMPONENTS
	NOTE: Functions will be added in a future update

	A. WORLD OBJECTS
		I. VARIABLES
			- title (string): The title used for searching for this specific world object
			- nodes (List): A list of nodes within the current world. This should not be manually touched, but is there for debugging.

		II. FUNCTIONS
			- None

	B. WORLD NODE OBJECTS
		I. VARIABLES
			- title (string): The title used for searching for this specific world node
			- sceneList (List): A list of NodeSceneReference Objects for the current world node.
				- NodeSceneReference Objects are used to contain multiple variations of a scene within the same node
				- NodeSceneReference Variables
					- timeSlot (string): The title used to determine the correct scene to use
					- scene (SceneAsset): The desired scene to be used
					- sceneName (string): If scene if null, use this to search for a scene

		II. FUNCTIONS
			- None

	C. WORLD NODE CONNECTIONS
		I. VARIABLES
			- title (string): The title used by the entrance node to determine which connection is being used, and used by the destination node to determine which entrance to put the player at in the new scene
			- reqItems (string list): A list of item titles that the connection requires to be unlocked/used. Only referenced if isLocked is set to true
			- isLocked (bool) [May be changed!]: Current locked state of the connection
			- lockedText (string) [May be changed!]: The text displayed when the door is locked and the player can not open it

		II. FUNCTIONS
			- None