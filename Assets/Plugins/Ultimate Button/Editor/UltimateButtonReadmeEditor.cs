/* Written by Kaz Crowe */
/* UltimateButtonReadmeEditor.cs */
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
[CustomEditor( typeof( UltimateButtonReadme ) )]
public class UltimateButtonReadmeEditor : Editor
{
	// LAYOUT STYLES //
	string Indent
	{
		get
		{
			return "    ";
		}
	}
	int sectionSpace = 20;
	int itemHeaderSpace = 10;
	int paragraphSpace = 5;
	GUIStyle titleStyle = new GUIStyle();
	GUIStyle sectionHeaderStyle = new GUIStyle();
	GUIStyle itemHeaderStyle = new GUIStyle();
	GUIStyle paragraphStyle = new GUIStyle();
	GUIStyle versionStyle = new GUIStyle();
	static string menuTitle = "Product Manual";

	class PageInformation
	{
		public string pageName = "";
		public Vector2 scrollPosition = Vector2.zero;
		public delegate void TargetMethod ();
		public TargetMethod targetMethod;
	}
	static PageInformation mainMenu = new PageInformation() { pageName = "Product Manual" };
	static PageInformation gettingStarted = new PageInformation() { pageName = "Getting Started" };
	static PageInformation overview = new PageInformation() { pageName = "Overview" };
	static PageInformation documentation = new PageInformation() { pageName = "Documentation" };
	static PageInformation versionHistory = new PageInformation() { pageName = "Version History" };
	static PageInformation importantChange = new PageInformation() { pageName = "Important Change" };
	static PageInformation thankYou = new PageInformation() { pageName = "Thank You!" };
	static PageInformation javascriptUsers = new PageInformation() { pageName = "JavaScript" };
	static List<PageInformation> pageHistory = new List<PageInformation>();
	static PageInformation currentPage = new PageInformation();

	class EndPageComment
	{
		public string comment = "";
		public string url = "";
	}
	EndPageComment[] endPageComments = new EndPageComment[]
	{
		new EndPageComment()
		{
			comment = "Enjoying the Ultimate Button? Leave us a review on the <b><color=blue>Unity Asset Store</color></b>!",
			url = "https://assetstore.unity.com/packages/slug/28824"
		},
		new EndPageComment()
		{
			comment = "Looking for a radial menu for your game? Check out the <b><color=blue>Ultimate Radial Menu</color></b>!",
			url = "https://www.tankandhealerstudio.com/ultimate-radial-menu.html"
		},
		new EndPageComment()
		{
			comment = "Looking for a health bar for your game? Check out the <b><color=blue>Simple Health Bar FREE</color></b>!",
			url = "https://www.tankandhealerstudio.com/simple-health-bar-free.html"
		},
		new EndPageComment()
		{
			comment = "Check out our <b><color=blue>other products</color></b>!",
			url = "https://www.tankandhealerstudio.com/assets.html"
		},
	};
	int randomComment = 0;
	static UltimateButtonReadme readme;

	bool navigateToVersionHistory = false;
	static int notifyImportantChange = 3;// UPDATE ON IMPORTANT CHANGES // 3 >= 2.6 / 2 >= 2.5 / 1 > ?
	class DocumentationInfo
	{
		public string functionName = "";
		public bool targetShowMore = false;
		public bool showMore = false;
		public string[] parameter;
		public string returnType = "";
		public string description = "";
		public string codeExample = "";
	}
	DocumentationInfo[] StaticFunctions = new DocumentationInfo[]
	{
		// GetUltimateButton
		new DocumentationInfo
		{
			functionName = "GetUltimateButton()",
			parameter = new string[ 1 ]
			{
				"string buttonName - The name that the targeted Ultimate Button has been registered with."
			},
			returnType = "UltimateButton",
			description = "Returns the Ultimate Button registered with the buttonName string. This function can be used to call local functions on the Ultimate Button to apply color changes or position updates at runtime.",
			codeExample = "UltimateButton jumpButton = UltimateButton.GetUltimateButton( \"Jump\" );"
		},
		// GetButtonDown
		new DocumentationInfo
		{
			functionName = "GetButtonDown()",
			parameter = new string[ 1 ]
			{
				"string buttonName - The name that the targeted Ultimate Button has been registered with."
			},
			description = "Returns true on the frame that the targeted Ultimate Button is pressed down.",
			codeExample = "if( UltimateButton.GetButtonDown( \"Jump\" ) )\n{\n    Debug.Log( \"The user has touched down on the jump button!\" );\n}"
		},
		// GetButtonUp
		new DocumentationInfo
		{
			functionName = "GetButtonUp()",
			parameter = new string[ 1 ]
			{
				"string buttonName - The name that the targeted Ultimate Button has been registered with."
			},
			description = "Returns true on the frame that the targeted Ultimate Button is released.",
			codeExample = "if( UltimateButton.GetButtonUp( \"Jump\" ) )\n{\n    Debug.Log( \"The user has released the touch on the jump button!\" );\n}"
		},
		// GetButton
		new DocumentationInfo
		{
			functionName = "GetButton()",
			parameter = new string[ 1 ]
			{
				"string buttonName - The name that the targeted Ultimate Button has been registered with."
			},
			description = "Returns true on the frames that the targeted Ultimate Button is being interacted with.",
			codeExample = "if( UltimateButton.GetButton( \"Jump\" ) )\n{\n    Debug.Log( \"The user is touching the jump button!\" );\n}"
		},
		// GetTapCount
		new DocumentationInfo
		{
			functionName = "GetTapCount()",
			parameter = new string[ 1 ]
			{
				"string buttonName - The name that the targeted Ultimate Button has been registered with."
			},
			description = "Returns true on the frame that the targeted Ultimate Button has achieved the tap count.",
			codeExample = "if( UltimateButton.GetTapCount( \"Jump\" ) )\n{\n    Debug.Log( \"The user has double tapped the jump button!\" );\n}"
		},
		// DisableButton
		new DocumentationInfo
		{
			functionName = "DisableButton()",
			parameter = new string[ 1 ]
			{
				"string buttonName - The name that the targeted Ultimate Button has been registered with."
			},
			description = "This function will reset the Ultimate Button and disable the gameObject. Use this function when wanting to disable the Ultimate Button from being used.",
			codeExample = "UltimateButton.DisableButton( \"Jump\" );"
		},
		// EnableButton
		new DocumentationInfo
		{
			functionName = "EnableButton()",
			parameter = new string[ 1 ]
			{
				"string buttonName - The name that the targeted Ultimate Button has been registered with."
			},
			description = "This function will ensure that the Ultimate Button is completely reset before enabling itself to be used again.",
			codeExample = "UltimateButton.EnableButton( \"Jump\" );"
		}
	};
	DocumentationInfo[] PublicFunctions = new DocumentationInfo[]
	{
		// UpdatePositioning
		new DocumentationInfo
		{
			functionName = "UpdatePositioning()",
			description = "Updates the size and positioning of the Ultimate Button. This function can be used to update any options that may have been changed prior to Start().",
			codeExample = "button.buttonSize = 4.0f;\nbutton.UpdatePositioning();"
		},
		// GetButtonDown
		new DocumentationInfo
		{
			functionName = "GetButtonDown()",
			description = "Returns true on the frame that the Ultimate Button is pressed down.",
			codeExample = "if( button.GetButtonDown( \"Jump\" ) )\n{\n    Debug.Log( \"The user has touched down on the jump button!\" );\n}"
		},
		// GetButton
		new DocumentationInfo
		{
			functionName = "GetButton()",
			description = "Returns true on the frames that the Ultimate Button is being interacted with.",
			codeExample = "if( button.GetButton( \"Jump\" ) )\n{\n    Debug.Log( \"The user is touching the jump button!\" );\n}"
		},
		// GetButtonUp
		new DocumentationInfo
		{
			functionName = "GetButtonUp()",
			description = "Returns true on the frame that the targeted Ultimate Button is released.",
			codeExample = "if( button.GetButtonUp( \"Jump\" ) )\n{\n    Debug.Log( \"The user has released the touch on the jump button!\" );\n}"
		},
		// GetTapCount
		new DocumentationInfo
		{
			functionName = "GetTapCount()",
			description = "Returns true when the Tap Count option has been achieved.",
			codeExample = "if( button.GetTapCount( \"Jump\" ) )\n{\n    Debug.Log( \"The user has double tapped the jump button!\" );\n}"
		},
		// DisableButton
		new DocumentationInfo 
		{
			functionName = "DisableButton()",
			description = "This function will reset the Ultimate Button and disable the gameObject. Use this function when wanting to disable the Ultimate Button from being used.",
			codeExample = "button.DisableButton();"
		},
		// EnableButton
		new DocumentationInfo
		{
			functionName = "EnableButton()",
			description = "This function will ensure that the Ultimate Button is completely reset before enabling itself to be used again.",
			codeExample = "button.EnableButton();"
		}
	};
	

	void OnEnable ()
	{
		if( !pageHistory.Contains( mainMenu ) )
			pageHistory.Insert( 0, mainMenu );

		mainMenu.targetMethod = MainPage;
		gettingStarted.targetMethod = GettingStarted;
		overview.targetMethod = Overview;
		documentation.targetMethod = Documentation;
		versionHistory.targetMethod = VersionHistory;
		importantChange.targetMethod = ImportantChange;
		thankYou.targetMethod = ThankYou;
		javascriptUsers.targetMethod = JavaScriptUsers;

		if( pageHistory.Count == 1 )
			currentPage = mainMenu;

		randomComment = Random.Range( 0, endPageComments.Length );

		var ids = AssetDatabase.FindAssets( "README t:UltimateButtonReadme" );
		if( ids.Length == 1 )
		{
			var readmeObject = AssetDatabase.LoadMainAssetAtPath( AssetDatabase.GUIDToAssetPath( ids[ 0 ] ) );

			readme = ( UltimateButtonReadme )readmeObject;
		}
	}

	protected override void OnHeaderGUI ()
	{
		UltimateButtonReadme readme = ( UltimateButtonReadme )target;

		var iconWidth = Mathf.Min( EditorGUIUtility.currentViewWidth, 350f );

		Vector2 ratio = new Vector2( readme.icon.width, readme.icon.height ) / ( readme.icon.width > readme.icon.height ? readme.icon.width : readme.icon.height );

		GUILayout.BeginHorizontal( "In BigTitle" );
		{
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical();
			GUILayout.Label( readme.icon, GUILayout.Width( iconWidth * ratio.x ), GUILayout.Height( iconWidth * ratio.y ) );
			GUILayout.Space( -20 );
			GUILayout.Label( readme.Version, versionStyle );
			var rect = GUILayoutUtility.GetLastRect();
			EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
			if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) && !pageHistory.Contains( versionHistory ) )
				navigateToVersionHistory = true;
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
		}
		GUILayout.EndHorizontal();
	}

	public override void OnInspectorGUI ()
	{
		//base.OnInspectorGUI();
		paragraphStyle = new GUIStyle( EditorStyles.label ) { wordWrap = true, richText = true, fontSize = 12 };
		itemHeaderStyle = new GUIStyle( paragraphStyle ) { fontSize = 12, fontStyle = FontStyle.Bold };
		sectionHeaderStyle = new GUIStyle( paragraphStyle ) { fontSize = 14, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
		titleStyle = new GUIStyle( paragraphStyle ) { fontSize = 16, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
		versionStyle = new GUIStyle( paragraphStyle ) { alignment = TextAnchor.MiddleCenter, fontSize = 10 };

		EditorGUILayout.BeginHorizontal();
		if( pageHistory.Count > 1 )
		{
			if( GUILayout.Button( "<", GUILayout.Width( 20 ) ) )
				NavigateBack();
		}
		GUILayout.FlexibleSpace();
		EditorGUILayout.LabelField( menuTitle, titleStyle );
		GUILayout.FlexibleSpace();
		if( pageHistory.Count > 1 )
			GUILayout.Space( 20 );

		EditorGUILayout.EndHorizontal();

		if( currentPage.targetMethod != null )
			currentPage.targetMethod();

		if( navigateToVersionHistory )
		{
			navigateToVersionHistory = false;
			NavigateForward( versionHistory );
		}

		Repaint();
	}

	void StartPage ( PageInformation pageInfo )
	{
		pageInfo.scrollPosition = EditorGUILayout.BeginScrollView( pageInfo.scrollPosition, false, false );
		GUILayout.Space( 15 );
	}

	void EndPage ()
	{
		EditorGUILayout.EndScrollView();
	}

	static void NavigateBack ()
	{
		pageHistory.RemoveAt( pageHistory.Count - 1 );
		menuTitle = pageHistory[ pageHistory.Count - 1 ].pageName;
		currentPage = pageHistory[ pageHistory.Count - 1 ];
	}

	static void NavigateForward ( PageInformation menu )
	{
		pageHistory.Add( menu );
		menuTitle = menu.pageName;
		currentPage = menu;
	}

	void MainPage ()
	{
		EditorGUILayout.LabelField( "We hope that you are enjoying using the Ultimate Button in your project!", paragraphStyle );
		EditorGUILayout.Space();
		EditorGUILayout.LabelField( "As with any package, you may be having some trouble understanding how to get the Ultimate Button working in your project. If so, have no fear, Tank & Healer Studio is here! Here is a few things that can help you get started:", paragraphStyle );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "  • Read the <b><color=blue>Getting Started</color></b> section of this README!", paragraphStyle );
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
			NavigateForward( gettingStarted );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "  • To learn more about the options on the inspector, read the <b><color=blue>Overview</color></b> section!", paragraphStyle );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
			NavigateForward( overview );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "  • Check out the <b><color=blue>Documentation</color></b> section!", paragraphStyle );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
			NavigateForward( documentation );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "  • Watch our <b><color=blue>Video Tutorials</color></b> on the Ultimate Button!", paragraphStyle );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
		{
			Debug.Log( "Ultimate Button\nOpening YouTube Tutorials" );
			Application.OpenURL( "https://www.youtube.com/playlist?list=PL7crd9xMJ9Tm14vBil6-DwaL0Ucip_buC" );
		}

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "  • <b><color=blue>Contact Us</color></b> directly with your issue! We'll try to help you out as much as we can.", paragraphStyle );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
		{
			Debug.Log( "Ultimate Button\nOpening Online Contact Form" );
			Application.OpenURL( "https://www.tankandhealerstudio.com/contact-us.html" );
		}

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "  • <b><color=blue>JavaScript Users</color></b> click here.", paragraphStyle );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
			NavigateForward( javascriptUsers );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "Now you have the tools you need to get the Ultimate Button working in your project. Now get out there and make your awesome game!", paragraphStyle );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "Happy Game Making,\n" + Indent + "Tank & Healer Studio", paragraphStyle );

		EditorGUILayout.Space();

		GUILayout.FlexibleSpace();

		EditorGUILayout.LabelField( endPageComments[ randomComment ].comment, paragraphStyle );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
			Application.OpenURL( endPageComments[ randomComment ].url );
	}

	void GettingStarted ()
	{
		StartPage( gettingStarted );

		EditorGUILayout.LabelField( "How To Create", sectionHeaderStyle );

		EditorGUILayout.LabelField( Indent + "To create an Ultimate Button in your scene, simply find the Ultimate Button prefab that you would like to add and drag it into your scene. What this does is creates that Ultimate Button within the scene and ensures that there is a Canvas and an EventSystem so that it can work correctly. If these are not present in the scene, they will be created for you.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "How To Reference", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "One of the great things about the Ultimate Button is how easy it is to reference to other scripts. The first thing that you will want to make sure to do is determine how you want to use the Ultimate Button within your scripts. If you are used to using the Events that are used in Unity's default UI buttons, then you may want to use the Unity Events options located within the Button Events section of the Ultimate Button inspector. However, if you are used to using Unity's Input system for getting input, then the Script Reference section would probably suit you better.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "For this example, we'll go over how to use the Script Reference section. First thing to do is assign the Button Name within the Script Reference section. After this is complete, you will be able to reference that particular button by it's name from a static function within the Ultimate Button script.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Example", sectionHeaderStyle );

		EditorGUILayout.LabelField( Indent + "Let's assume you are going to use the Ultimate Button for making a player jump. You will need to check the button's state to determine when the user has touched the button and is wanting the player to jump. So for this example, let's assign the name \"Jump\" in the Script Reference section of the Ultimate Button.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		Vector2 ratio = new Vector2( readme.scriptReference.width, readme.scriptReference.height ) / ( readme.scriptReference.width > readme.scriptReference.height ? readme.scriptReference.width : readme.scriptReference.height );

		float imageWidth = readme.scriptReference.width > Screen.width - 50 ? Screen.width - 50 : readme.scriptReference.width;

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label( readme.scriptReference, GUILayout.Width( imageWidth ), GUILayout.Height( imageWidth * ratio.y ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "There are several functions that allow you to check the different states that the Ultimate Button is in. For more information on all the functions that you have available to you, please see the documentation section of this README.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "For this example we will be using the GetButtonDown function to see if the user has pressed down on the button. It is worth noting that this function is useful when wanting to make the player start the jump action on the exact frame that the user has pressed down on the button, and not after.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "Example Code:", itemHeaderStyle );
		EditorGUILayout.TextArea( "if( UltimateButton.GetButtonDown( \"Jump\" ) )\n{\n	// Call player jump function.\n}", GUI.skin.GetStyle( "TextArea" ) );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "Feel free to experiment with the different functions of the Ultimate Button to get it working exactly the way you want to. Additionally, if you are curious about how the Ultimate Button has been implemented into an Official Tank and Healer Studio example, then please see the README_TruckController.txt that is included with the example files for the project.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EndPage();
	}

	void Overview ()
	{
		StartPage( overview );

		/* //// --------------------------- < SIZE AND PLACEMENT > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Size And Placement", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "The Size and Placement section allows you to customize the button's size and placement on the screen, as well as determine where the user's touch can be processed for the selected button.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Positioning
		EditorGUILayout.LabelField( "Positioning", itemHeaderStyle );
		EditorGUILayout.LabelField( "Determines how the Ultimate Button will be positioned on the screen. The Screen Space option will position the button according to the available screen space, while the Relative To Transform option will position the button in relative position to another UI GameObject.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Scaling Axis
		EditorGUILayout.LabelField( "Scaling Axis (Screen Space)", itemHeaderStyle );
		EditorGUILayout.LabelField( "Determines which axis the button will be scaled from. If Height is chosen, then the button will scale itself proportionately to the Height of the screen.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Anchor
		EditorGUILayout.LabelField( "Anchor (Screen Space)", itemHeaderStyle );
		EditorGUILayout.LabelField( "Determines which side of the screen that the button will be anchored to.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Touch Size
		EditorGUILayout.LabelField( "Touch Size (Screen Space)", itemHeaderStyle );
		EditorGUILayout.LabelField( "Touch Size configures the size of the area where the user can touch. You have the options of either 'Default','Medium', or 'Large'.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Relative Transform
		EditorGUILayout.LabelField( "Relative Space (Relative To Transform)", itemHeaderStyle );
		EditorGUILayout.LabelField( "The space reserved for positioning this button relative to the target transform's size.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Relative Transform
		EditorGUILayout.LabelField( "Relative Transform (Relative To Transform)", itemHeaderStyle );
		EditorGUILayout.LabelField( "The RectTransform component to position this button relative to.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Button Size
		EditorGUILayout.LabelField( "Button Size", itemHeaderStyle );
		EditorGUILayout.LabelField( "Button Size will change the scale of the button. Since everything is calculated out according to screen size, your Touch Size option and other properties will scale proportionately with the button's size along your specified Scaling Axis.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Button Position
		EditorGUILayout.LabelField( "Button Position", itemHeaderStyle );
		EditorGUILayout.LabelField( "Button Position will present you with two sliders. The X value will determine how far the button is away from the Left and Right sides of the screen, and the Y value from the Top and Bottom. This will encompass 50% of your screen, relevant to your Anchor selection.", paragraphStyle );
		/* \\\\ -------------------------- < END SIZE AND PLACEMENT > --------------------------- //// */

		GUILayout.Space( sectionSpace );

		/* //// ----------------------------- < BUTTON FUNCTIONALITY > ----------------------------- \\\\ */
		EditorGUILayout.LabelField( "Button Functionality", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "The Button Functionality section contains options that affect how the button functions.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Image Style
		EditorGUILayout.LabelField( "Image Style", itemHeaderStyle );
		EditorGUILayout.LabelField( "Determines whether the input range should be circular or square. This option affects how the Input Range and Track Input options function.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Input Range
		EditorGUILayout.LabelField( "Input Range", itemHeaderStyle );
		EditorGUILayout.LabelField( "The range that the Ultimate Button will react to when initiating and dragging the input.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Track Input
		EditorGUILayout.LabelField( "Track Input", itemHeaderStyle );
		EditorGUILayout.LabelField( "If the Track Input option is enabled, then the Ultimate Button will reflect it's state according to where the user's input currently is. This means that if the input moves off of the button, then the button state will turn to false. When the input returns to the button the state will return to true. If the Track Input option is disabled, then the button will reflect the state of only pressing and releasing the button.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Transmit Input
		EditorGUILayout.LabelField( "Transmit Input", itemHeaderStyle );
		EditorGUILayout.LabelField( "The Transmit Input option will allow you to send the input data to another script that uses Unity's EventSystem. For example, if you are using the Ultimate Joystick package, you could place the Ultimate Button on top of the Ultimate Joystick, and still have the Ultimate Button and Ultimate Joystick function correctly when interacted with.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Tap Count Option
		EditorGUILayout.LabelField( "Tap Count Option", itemHeaderStyle );
		EditorGUILayout.LabelField( "The Tap Count option allows you to decide if you want to store the amount of taps that the button receives. The options provided with the Tap Count will allow you to customize the target amount of taps, the tap time window, and the event to be called when the tap count has been achieved.", paragraphStyle );
		/* //// --------------------------- < END BUTTON FUNCTIONALITY > --------------------------- \\\\ */

		GUILayout.Space( sectionSpace );

		/* //// ----------------------------- < VISUAL OPTIONS > ----------------------------- \\\\ */
		EditorGUILayout.LabelField( "Visual Options", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "The Visual Options section contains options that affect how the button is visually presented to the user.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Base Color
		EditorGUILayout.LabelField( "Base Color", itemHeaderStyle );
		EditorGUILayout.LabelField( "The Base Color option determines the color of the button base images.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Show Highlight
		EditorGUILayout.LabelField( "Show Highlight", itemHeaderStyle );
		EditorGUILayout.LabelField( "Show Highlight will allow you to customize the set highlight images with a custom color. With this option, you will also be able to customize and set these images at runtime using the UpdateHighlightColor function. See the Documentation section for more details.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Show Tension
		EditorGUILayout.LabelField( "Show Tension", itemHeaderStyle );
		EditorGUILayout.LabelField( "With Show Tension enabled, the button will display interactions visually using custom colors and images to display the intensity of the press. With this option enabled, you will be able to update the tension colors at runtime using the UpdateTensionColors function. See the Documentation section for more information.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Use Animation
		EditorGUILayout.LabelField( "Use Animation", itemHeaderStyle );
		EditorGUILayout.LabelField( "If you would like the button to play an animation when being interacted with, then you will want to enable the Use Animation option.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Use Fade
		EditorGUILayout.LabelField( "Use Fade", itemHeaderStyle );
		EditorGUILayout.LabelField( "The Use Fade option will present you with settings for the targeted alpha for the touched and untouched states, as well as the duration for the fade between the targeted alpha settings.", paragraphStyle );
		/* //// --------------------------- < END VISUAL OPTIONS > --------------------------- \\\\ */


		GUILayout.Space( sectionSpace );

		/* //// ----------------------------- < SCRIPT REFERENCE > ------------------------------ \\\\ */
		EditorGUILayout.LabelField( "Script Reference", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "The Script Reference section contains fields for naming and helpful code snippets that you can copy and paste into your scripts. Be sure to refer to the README for information about the functions that you have available to you.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Button Name
		EditorGUILayout.LabelField( "Button Name", itemHeaderStyle );
		EditorGUILayout.LabelField( "The unique name of your Ultimate Button. This name is what will be used to reference this particular button from the public static functions.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// Example Code
		EditorGUILayout.LabelField( "Example Code Generator", itemHeaderStyle );
		EditorGUILayout.LabelField( "This section will present you with code snippets that are determined by your selection. This code can be copy and pasted into your custom scripts. Please note that this section is only designed to help you get the Ultimate Button working in your scripts quickly. Any options within this section do have affect the actual functionality of the button.", paragraphStyle );
		/* //// --------------------------- < END SCRIPT REFERENCE > ---------------------------- \\\\ */

		GUILayout.Space( sectionSpace );

		/* //// ------------------------------- < BUTTON EVENTS > ------------------------------- \\\\ */
		EditorGUILayout.LabelField( "Button Events", sectionHeaderStyle );
		EditorGUILayout.LabelField( Indent + "The Button Events section contains Unity Events that can be created for when the Ultimate Button is pressed and released. Also, if you have the Tap Count Option set, then you can assign a Unity Event for the Tap Count Event option.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );
		/* //// ----------------------------- < END BUTTON EVENTS > ----------------------------- \\\\ */

		EndPage();
	}

	void Documentation ()
	{
		StartPage( documentation );

		/* //// --------------------------- < STATIC FUNCTIONS > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Static Functions", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "The following functions can be referenced from your scripts without the need for an assigned local Ultimate Button variable. However, each function must have the targeted Ultimate Button name in order to find the correct Ultimate Button in the scene. Each example code provided uses the name 'Jump' as the button name.", paragraphStyle );

		Vector2 ratio = new Vector2( readme.scriptReference.width, readme.scriptReference.height ) / ( readme.scriptReference.width > readme.scriptReference.height ? readme.scriptReference.width : readme.scriptReference.height );

		float imageWidth = readme.scriptReference.width > Screen.width - 50 ? Screen.width - 50 : readme.scriptReference.width;

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label( readme.scriptReference, GUILayout.Width( imageWidth ), GUILayout.Height( imageWidth * ratio.y ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.LabelField( "Please click on the function name to learn more.", paragraphStyle );

		for( int i = 0; i < StaticFunctions.Length; i++ )
			ShowDocumentation( StaticFunctions[ i ] );

		GUILayout.Space( sectionSpace );

		/* //// --------------------------- < PUBLIC FUNCTIONS > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Public Functions", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "All of the following public functions are only available from a reference to the Ultimate Button. Each example provided relies on having a Ultimate Button variable named 'button' stored inside your script. When using any of the example code provided, make sure that you have a public Ultimate Button variable like the one below:", paragraphStyle );

		EditorGUILayout.TextArea( "public UltimateButton button;", GUI.skin.textArea );

		GUILayout.Space( paragraphSpace );

		for( int i = 0; i < PublicFunctions.Length; i++ )
			ShowDocumentation( PublicFunctions[ i ] );

		GUILayout.Space( itemHeaderSpace );

		EndPage();
	}

	void VersionHistory ()
	{
		StartPage( versionHistory );

		EditorGUILayout.LabelField( "Version 2.6.1", itemHeaderStyle );
		EditorGUILayout.LabelField( "  • Updated the Truck example scene to remove a warning about the JointMotor2D in Unity 2018+. Also modified the order value for the truck to look correct.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Updated script reference section of the Ultimate Button to not include the if() conditional to copy and paste. This simplifies the reference and makes it easier to use.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Version 2.6.0 (The Return of Ultimate Button)", itemHeaderStyle );
		EditorGUILayout.LabelField( "  • Improved the Ultimate Button textures.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Removed AnimBool functionality from the inspector to avoid errors with Unity 2019+.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Added a new positioning option for Relative to Transform.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Added new script: UltimateButtonReadme.cs.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Added new script: UltimateButtonReadmeEditor.cs.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Added new file at the Ultimate Button root folder: README. This file has all the documentation and how to information.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Removed the UltimateButtonWindow.cs file. All of that information is now located in the README file.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Removed the old README text file. All of that information is now located in the README file.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Removed several useless public functions: UpdateBaseColor, UpdateHighlightColor, and UpdateTensionColor. Even without these functions you can still modify the corresponding variables easily to get the same functionality.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Added several new public functions for reference: GetButtonDown, GetButton, GetButtonUp, GetTapCount.", paragraphStyle );

		EndPage();
	}

	void ImportantChange ()
	{
		StartPage( importantChange );

		EditorGUILayout.LabelField( Indent + "Thank you for downloading the most recent version of the Ultimate Button. If you are experiencing any errors, please completely remove the Ultimate Button from your project and re-import it. As always, if you run into any issues with the Ultimate Button, please contact us at:", paragraphStyle );

		GUILayout.Space( paragraphSpace );
		EditorGUILayout.SelectableLabel( "tankandhealerstudio@outlook.com", itemHeaderStyle, GUILayout.Height( 15 ) );
		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "NEW FILES", sectionHeaderStyle );
		EditorGUILayout.LabelField( "  • UltimateButtonReadme.cs", paragraphStyle );
		EditorGUILayout.LabelField( "  • UltimateButtonReadmeEditor.cs", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "OLD FILES", sectionHeaderStyle );
		EditorGUILayout.LabelField( "The file listed below is not longer used, and can (and should) be removed from your project. All the information that was previously inside this script is now included in the Ultimate Button README.", paragraphStyle );
		EditorGUILayout.LabelField( "  • UltimateButtonWindow.cs", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "REMOVED PUBLIC FUNCTIONS", sectionHeaderStyle );
		EditorGUILayout.LabelField( "  • UpdateBaseColor", paragraphStyle );
		EditorGUILayout.LabelField( "  • UpdateHighlightColor", paragraphStyle );
		EditorGUILayout.LabelField( "  • UpdateTensionColor", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "NEW PUBLIC FUNCTIONS", sectionHeaderStyle );
		EditorGUILayout.LabelField( "  • GetButtonDown", paragraphStyle );
		EditorGUILayout.LabelField( "  • GetButton", paragraphStyle );
		EditorGUILayout.LabelField( "  • GetButtonUp", paragraphStyle );
		EditorGUILayout.LabelField( "  • GetTapCount", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Got it!", GUILayout.Width( Screen.width / 2 ) ) )
			NavigateBack();

		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EndPage();
	}

	void ThankYou ()
	{
		StartPage( thankYou );

		EditorGUILayout.LabelField( "The two of us at Tank & Healer Studio would like to thank you for purchasing the Ultimate Button asset package from the Unity Asset Store.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "We hope that the Ultimate Button will be a great help to you in the development of your game. Here is a few things that can help you get started:", paragraphStyle );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "  • Read the <b><color=blue>Getting Started</color></b> section of this README!", paragraphStyle );
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
			NavigateForward( gettingStarted );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "  • To learn more about the options on the inspector, read the <b><color=blue>Overview</color></b> section!", paragraphStyle );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
			NavigateForward( overview );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "  • Check out the <b><color=blue>Documentation</color></b> section to learn more about how to use the Ultimate Button in your scripts!", paragraphStyle );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
			NavigateForward( documentation );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "You can access this information at any time by clicking on the <b>README</b> file inside the Ultimate Button folder.", paragraphStyle );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "Again, thank you for downloading the Ultimate Button. We hope that your project is a success!", paragraphStyle );

		EditorGUILayout.Space();

		EditorGUILayout.LabelField( "Happy Game Making,\n" + Indent + "Tank & Healer Studio", paragraphStyle );

		GUILayout.Space( 15 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Continue", GUILayout.Width( Screen.width / 2 ) ) )
			NavigateBack();

		var rect2 = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect2, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EndPage();
	}

	void JavaScriptUsers ()
	{
		StartPage( javascriptUsers );

		EditorGUILayout.LabelField( Indent + "If you are using Javascript to program your game, you may be unable to reference the Ultimate Button in it's current folder structure.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "In order for us to upload this package to the Unity Asset Store, we are required to put all the files within a single sub-folder. This means that we cannot have a Plugins folder. For more information about the Plugins folder and script compilation order, please look into Unity's documentation.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "In short, C# scripts are compiled in a different pass than Javascript, which means that Javascript cannot reference C# scripts without them being placed in a special folder. In order to reference the Ultimate Button script from Javascript, simply create a folder in your main Assets folder named <b>Plugins</b>, and place the UltimateButton.cs script into the <b>Plugins</b> folder.", paragraphStyle );

		EndPage();
	}
	
	void ShowDocumentation ( DocumentationInfo info )
	{
		GUILayout.Space( paragraphSpace );
		
		EditorGUILayout.LabelField( info.functionName, itemHeaderStyle );
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
			info.targetShowMore = !info.targetShowMore;

		if( info.targetShowMore != info.showMore && Event.current.type == EventType.Layout )
			info.showMore = info.targetShowMore;
		
		if( info.showMore )
		{
			EditorGUILayout.LabelField( Indent + "<i>Description:</i> " + info.description, paragraphStyle );

			if( info.parameter != null )
			{
				for( int i = 0; i < info.parameter.Length; i++ )
					EditorGUILayout.LabelField( Indent + "<i>Parameter:</i> " + info.parameter[ i ], paragraphStyle );
			}
			if( info.returnType != string.Empty )
				EditorGUILayout.LabelField( Indent + "<i>Return type:</i> " + info.returnType, paragraphStyle );

			if( info.codeExample != string.Empty )
				EditorGUILayout.TextArea( info.codeExample, GUI.skin.textArea );

			GUILayout.Space( paragraphSpace );
		}
	}

	public static void OpenReadmeDocumentation ()
	{
		var ids = AssetDatabase.FindAssets( "README t:UltimateButtonReadme" );
		if( ids.Length == 1 )
		{
			var readmeObject = AssetDatabase.LoadMainAssetAtPath( AssetDatabase.GUIDToAssetPath( ids[ 0 ] ) );
			Selection.objects = new Object[] { readmeObject };
			readme = ( UltimateButtonReadme )readmeObject;

			if( !pageHistory.Contains( documentation ) )
				NavigateForward( documentation );
		}
		else
		{
			Debug.LogError( "There is no README object in the Ultimate Button folder." );
		}
	}

	[InitializeOnLoad]
	class UltimateButtonInitialLoad
	{
		static UltimateButtonInitialLoad ()
		{
			// If the user has a older version of UJ that used the bool for startup...
			if( EditorPrefs.HasKey( "UltimateButtonStartup" ) && !EditorPrefs.HasKey( "UltimateButtonVersion" ) )
			{
				// Set the new pref to 0 so that the pref will exist and the version changes will be shown.
				EditorPrefs.SetInt( "UltimateButtonVersion", 0 );
			}

			// If this is the first time that the user has downloaded the Ultimate Button...
			if( !EditorPrefs.HasKey( "UltimateButtonVersion" ) )
			{
				// Navigate to the Thank You page.
				NavigateForward( thankYou );

				// Set the version to current so they won't see these version changes.
				EditorPrefs.SetInt( "UltimateButtonVersion", notifyImportantChange );

				EditorApplication.update += WaitForCompile;
			}
			else if( EditorPrefs.GetInt( "UltimateButtonVersion" ) < notifyImportantChange )
			{
				// Navigate to the Version Changes page.
				NavigateForward( importantChange );

				// Set the version to current so they won't see this page again.
				EditorPrefs.SetInt( "UltimateButtonVersion", notifyImportantChange );

				EditorApplication.update += WaitForCompile;
			}

			var ids = AssetDatabase.FindAssets( "README t:UltimateButtonReadme" );
			if( ids.Length == 1 )
			{
				var readmeObject = AssetDatabase.LoadMainAssetAtPath( AssetDatabase.GUIDToAssetPath( ids[ 0 ] ) );
				readme = ( UltimateButtonReadme )readmeObject;
			}
		}

		static void WaitForCompile ()
		{
			if( EditorApplication.isCompiling )
				return;

			EditorApplication.update -= WaitForCompile;

			var ids = AssetDatabase.FindAssets( "README t:UltimateButtonReadme" );
			if( ids.Length == 1 )
			{
				var readmeObject = AssetDatabase.LoadMainAssetAtPath( AssetDatabase.GUIDToAssetPath( ids[ 0 ] ) );
				Selection.objects = new Object[] { readmeObject };
				readme = ( UltimateButtonReadme )readmeObject;
			}
		}
	}
}