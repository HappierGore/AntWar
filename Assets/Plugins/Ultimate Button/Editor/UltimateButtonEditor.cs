/* Written by Kaz Crowe */
/* UltimateButtonEditor.cs */
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[CanEditMultipleObjects]
[CustomEditor( typeof( UltimateButton ) )]
public class UltimateButtonEditor : Editor
{
	UltimateButton targ;

	/* ---< ASSIGNED VARIABLES >--- */
	SerializedProperty sizeFolder, buttonHighlight;
	SerializedProperty tensionAccent, buttonAnimator;
	SerializedProperty buttonBase;

	/* ---< SIZE AND PLACEMENT >--- */
	SerializedProperty positioning, relativeTransform;
	SerializedProperty scalingAxis, anchor, touchSize;
	SerializedProperty buttonSize, customSpacing_X, customSpacing_Y;

	/* ---< STYLES AND OPTIONS >--- */
	SerializedProperty imageStyle, inputRange;
	SerializedProperty trackInput, transmitInput, receiver;
	SerializedProperty tapCountOption, tapCountDuration, targetTapCount;

	SerializedProperty baseColor;
	SerializedProperty showHighlight, highlightColor;
	SerializedProperty showTension, tensionColorNone, tensionColorFull;
	SerializedProperty tensionFadeInDuration, tensionFadeOutDuration;
	SerializedProperty useAnimation, useFade;
	SerializedProperty fadeUntouched, fadeTouched;
	SerializedProperty fadeInDuration, fadeOutDuration;

	/* ---< SCRIPT REFERENCE >--- */
	SerializedProperty buttonName;

	// ----->>> EXAMPLE CODE //
	class ExampleCode
	{
		public string optionName = "";
		public string optionDescription = "";
		public string basicCode = "";
	}
	ExampleCode[] exampleCodes = new ExampleCode[]
	{
		new ExampleCode() { optionName = "GetButtonDown ( string buttonName )", optionDescription = "Returns true on the frame that the button was pressed down.", basicCode = "UltimateButton.GetButtonDown( \"{0}\" )" },
		new ExampleCode() { optionName = "GetButtonUp ( string buttonName )", optionDescription = "Returns true on the frame that the button was released.", basicCode = "UltimateButton.GetButtonUp( \"{0}\" )" },
		new ExampleCode() { optionName = "GetButton ( string buttonName )", optionDescription = "Returns the current state of the buttons state. True for being pressed down and false for no input.", basicCode = "UltimateButton.GetButton( \"{0}\" )" },
		new ExampleCode() { optionName = "GetTapCount ( string buttonName )", optionDescription = "Returns true when the user has achieved the tap count.", basicCode = "UltimateButton.GetTapCount( \"{0}\" )" },
		new ExampleCode() { optionName = "GetUltimateButton ( string buttonName )", optionDescription = "Returns the Ultimate Button component that has been registered with the targeted button name.", basicCode = "UltimateButton jumpButton = UltimateButton.GetUltimateButton( \"{0}\" );" },
		new ExampleCode() { optionName = "DisableButton ( string buttonName )", optionDescription = "Disables the Ultimate Button.", basicCode = "UltimateButton.DisableButton( \"{0}\" );" },
		new ExampleCode() { optionName = "EnableButton ( string buttonName )", optionDescription = "Enables the Ultimate Button.", basicCode = "UltimateButton.EnableButton( \"{0}\" );" },
	};
	List<string> exampleCodeOptions = new List<string>();
	int exampleCodeIndex = 0;

	/* ---< BUTTON EVENTS >--- */
	SerializedProperty onButtonDown;
	SerializedProperty onButtonUp;
	SerializedProperty tapCountEvent;

	/* ---< INTERNAL >--- */
	Canvas parentCanvas;

	int inputRangeHighlighted = 0;

	
	void OnEnable ()
	{
		// Store the Ultimate Button references as soon as this script is being viewed.
		StoreReferences();

		// Register the Undo function to be called for undo's.
		Undo.undoRedoPerformed += UndoRedoCallback;

		// Store the parent canvas.
		parentCanvas = GetParentCanvas();
	}

	void OnDisable ()
	{
		// Remove the UndoRedoCallback from the Undo event.
		Undo.undoRedoPerformed -= UndoRedoCallback;
	}

	Canvas GetParentCanvas ()
	{
		// If the current selection is null, then return.
		if( Selection.activeGameObject == null )
			return null;

		// Store the current parent.
		Transform parent = Selection.activeGameObject.transform.parent;

		// Loop through parents as long as there is one.
		while( parent != null )
		{ 
			// If there is a Canvas component, return the component.
			if( parent.transform.GetComponent<Canvas>() && parent.transform.GetComponent<Canvas>().enabled == true )
				return parent.transform.GetComponent<Canvas>();
			
			// Else, shift to the next parent.
			parent = parent.transform.parent;
		}
		if( parent == null && !AssetDatabase.Contains( Selection.activeGameObject ) )
			UltimateButtonCreator.RequestCanvas( Selection.activeGameObject );

		return null;
	}

	// Function for Undo/Redo operations.
	void UndoRedoCallback ()
	{
		// Re-reference all variables on undo/redo.
		StoreReferences();
	}

	// Function called to display an interactive header.
	void DisplayHeader ( string headerName, string editorPref )
	{
		EditorGUILayout.BeginVertical( "Toolbar" );
		GUILayout.BeginHorizontal();

		EditorGUILayout.LabelField( headerName, EditorStyles.boldLabel );
		if( GUILayout.Button( EditorPrefs.GetBool( editorPref ) == true ? "Hide" : "Show", EditorStyles.miniButton, GUILayout.Width( 50 ), GUILayout.Height( 14f ) ) )
			EditorPrefs.SetBool( editorPref, EditorPrefs.GetBool( editorPref ) == true ? false : true );

		GUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
	}

	bool CanvasErrors ()
	{
		// If the selection is actually the prefab within the Project window, then return no errors.
		if( AssetDatabase.Contains( Selection.activeGameObject ) )
			return false;

		// If parentCanvas is unassigned, then get a new canvas and return no errors.
		if( parentCanvas == null )
		{
			parentCanvas = GetParentCanvas();
			return false;
		}

		// If the parentCanvas is not enabled, then return true for errors.
		if( parentCanvas.enabled == false )
			return true;

		// If the canvas' renderMode is not the needed one, then return true for errors.
		if( parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay )
			return true;

		// If the canvas has a CanvasScaler component and it is not the correct option.
		if( parentCanvas.GetComponent<CanvasScaler>() && parentCanvas.GetComponent<CanvasScaler>().uiScaleMode != CanvasScaler.ScaleMode.ConstantPixelSize )
			return true;

		return false;
	}
	
	public override void OnInspectorGUI ()
	{
		serializedObject.Update();

		EditorGUILayout.Space();
		
		#region CANVAS ERRORS
		if( CanvasErrors() == true )
		{
			if( parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay )
			{
				EditorGUILayout.BeginVertical( "Box" );
				EditorGUILayout.HelpBox( "The parent Canvas needs to be set to 'Screen Space - Overlay' in order for the Ultimate Button to function correctly.", MessageType.Error );
				EditorGUILayout.BeginHorizontal();
				if( GUILayout.Button( "Update Canvas", EditorStyles.miniButtonLeft ) )
				{
					parentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
					parentCanvas = GetParentCanvas();
				}
				if( GUILayout.Button( "Update Button", EditorStyles.miniButtonRight ) )
				{
					UltimateButtonCreator.RequestCanvas( Selection.activeGameObject );
					parentCanvas = GetParentCanvas();
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
			}
			if( parentCanvas.GetComponent<CanvasScaler>() )
			{
				if( parentCanvas.GetComponent<CanvasScaler>().uiScaleMode != CanvasScaler.ScaleMode.ConstantPixelSize )
				{
					EditorGUILayout.BeginVertical( "Box" );
					EditorGUILayout.HelpBox( "The Canvas Scaler component located on the parent Canvas needs to be set to 'Constant Pixel Size' in order for the Ultimate Button to function correctly.", MessageType.Error );
					EditorGUILayout.BeginHorizontal();
					if( GUILayout.Button( "Update Canvas", EditorStyles.miniButtonLeft ) )
					{
						parentCanvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
						parentCanvas = GetParentCanvas();
						UltimateButton button = ( UltimateButton )target;
						button.UpdatePositioning();
					}
					if( GUILayout.Button( "Update Button", EditorStyles.miniButtonRight ) )
					{
						UltimateButtonCreator.RequestCanvas( Selection.activeGameObject );
						parentCanvas = GetParentCanvas();
					}
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical();
				}
			}
			return;
		}
		#endregion
		
		UltimateButton targ = ( UltimateButton ) target;
		
		#region SIZE AND PLACEMENT
		DisplayHeader( "Size and Placement", "UUI_SizeAndPlacement" );
		
		if( EditorPrefs.GetBool( "UUI_SizeAndPlacement" ) )
		{
			EditorGUILayout.Space();
			
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( positioning );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();

			EditorGUI.BeginDisabledGroup( targ.positioning == UltimateButton.Positioning.Disabled );
			{
				if( targ.positioning == UltimateButton.Positioning.RelativeToTransform )
				{
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( relativeTransform );
					EditorGUI.indentLevel = 1;
					EditorGUILayout.Slider( serializedObject.FindProperty( "relativeSpaceMod" ), 1.5f, 2.5f, new GUIContent( "Relative Space", "EDIT:" ) );
					EditorGUI.indentLevel = 0;
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();
				}

				if( targ.positioning == UltimateButton.Positioning.ScreenSpace )
				{
					EditorGUI.BeginChangeCheck();
					{
						EditorGUILayout.PropertyField( scalingAxis );
						EditorGUILayout.PropertyField( anchor );
						EditorGUILayout.PropertyField( touchSize, new GUIContent( "Touch Size", "The size of the area in which the touch can start" ) );
					}
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();
				}

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( sizeFolder, new GUIContent( "Button Size Folder" ) );
				EditorGUILayout.Slider( buttonSize, 0.0f, 5.0f, new GUIContent( "Button Size" ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();

				EditorGUILayout.BeginVertical( "Box" );
				{
					EditorGUILayout.LabelField( "Button Position" );
					EditorGUI.indentLevel = 1;
					{
						EditorGUI.BeginChangeCheck();
						{
							EditorGUILayout.Slider( customSpacing_X, 0.0f, targ.positioning == UltimateButton.Positioning.RelativeToTransform ? 100.0f : 50.0f, new GUIContent( "X Position:" ) );
							EditorGUILayout.Slider( customSpacing_Y, 0.0f, 100.0f, new GUIContent( "Y Position:" ) );
						}
						if( EditorGUI.EndChangeCheck() )
							serializedObject.ApplyModifiedProperties();
					}
					EditorGUI.indentLevel = 0;
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUI.EndDisabledGroup();
		}
		#endregion

		EditorGUILayout.Space();

		#region BUTTON FUNCTIONALITY
		DisplayHeader( "Button Functionality", "UUI_Functionality" );
		if( EditorPrefs.GetBool( "UUI_Functionality" ) )
		{
			EditorGUILayout.Space();

			if( targ.touchSize == UltimateButton.TouchSize.Default )
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( imageStyle, new GUIContent( "Image Style", "Determines whether the input range should be circular or square. This option affects how the Input Range and Track Input options function." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider( inputRange, 0.0f, 1.0f, new GUIContent( "Input Range", "The range that the Ultimate Button will react to when initiating and dragging the input." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					inputRangeHighlighted = 100;
				}

				if( Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains( Event.current.mousePosition ) && inputRangeHighlighted <= 5 )
					inputRangeHighlighted = 5;

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( trackInput, new GUIContent( "Track Input", "Enabling this option will allow the Ultimate Button to track the users input to ensure that button events and states are only called when the input is within the Input Range." ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();
			}

			// TRANSMIT INPUT //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( transmitInput, new GUIContent( "Transmit Input", "Should the Ultimate Button transmit input events to another UI game object?" ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();

			if( targ.transmitInput )
			{
				EditorGUI.indentLevel = 1;

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( receiver, new GUIContent( "Input Receiver" ) );
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();

				EditorGUI.indentLevel = 0;
				EditorGUILayout.Space();
			}

			// TAP COUNT OPTIONS //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( tapCountOption );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();

			if( targ.tapCountOption != UltimateButton.TapCountOption.NoCount )
			{
				EditorGUI.indentLevel = 1;
				EditorGUI.BeginChangeCheck();
				{
					EditorGUILayout.Slider( tapCountDuration, 0.0f, 1.0f, new GUIContent( "Tap Time Window" ) );
					EditorGUI.BeginDisabledGroup( targ.tapCountOption != UltimateButton.TapCountOption.Accumulate );
					EditorGUILayout.IntSlider( targetTapCount, 1, 5, new GUIContent( "Target Tap Count" ) );
					EditorGUI.EndDisabledGroup();
				}
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();

				EditorGUI.indentLevel = 0;

				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if( GUILayout.Button( "Example Code" ) )
				{
					EditorPrefs.SetBool( "UUI_ScriptReference", true );
					exampleCodeIndex = 3;
				}
				if( GUILayout.Button( "Button Events" ) )
					EditorPrefs.SetBool( "UUI_ExtraOption_01", true );

				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
			}
		}
		#endregion

		EditorGUILayout.Space();
		
		#region VISUAL OPTIONS
		DisplayHeader( "Visual Options", "UUI_VisualOptions" );
		if( EditorPrefs.GetBool( "UUI_VisualOptions" ) )
		{
			EditorGUILayout.Space();

			// BASE COLOR //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( buttonBase );
			EditorGUILayout.PropertyField( baseColor );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				if( targ.buttonBase != null )
				{
					targ.buttonBase.color = targ.baseColor;
					EditorUtility.SetDirty( targ.buttonBase );
				}
			}
			if( targ.buttonBase == null )
			{
				EditorGUI.indentLevel = 1;
				EditorGUILayout.HelpBox( "The Button Base image has not been assigned. Please make sure to assign this variable within the Assigned Variables section.", MessageType.Warning );
				EditorGUI.indentLevel = 0;
				EditorGUILayout.Space();
			}

			// --------------------------< HIGHLIGHT >-------------------------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( showHighlight, new GUIContent( "Show Highlight", "Displays the highlight images with the Highlight Color variable." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				SetHighlight();
				if( targ.buttonHighlight != null )
					EditorUtility.SetDirty( targ.buttonHighlight );
			}
			EditorGUI.indentLevel = 1;
			if( targ.showHighlight )
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( buttonHighlight, new GUIContent( "Button Highlight" ) );
				EditorGUILayout.PropertyField( highlightColor );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					if( targ.buttonHighlight != null )
					{
						targ.buttonHighlight.color = targ.highlightColor;
						EditorUtility.SetDirty( targ.buttonHighlight );
					}
				}
				EditorGUILayout.Space();
			}

			EditorGUI.indentLevel = 0;
			// ------------------------< END HIGHLIGHT >------------------------ //

			// ---------------------------< TENSION >--------------------------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( showTension, new GUIContent( "Show Tension", "Displays the visual state of the button using the tension color options." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				SetTensionAccent();
				if( targ.tensionAccent != null )
					EditorUtility.SetDirty( targ.tensionAccent );
			}

			EditorGUI.indentLevel = 1;

			if( targ.showTension )
			{
				EditorGUI.BeginChangeCheck();
				{
					EditorGUILayout.PropertyField( tensionAccent );
					EditorGUILayout.PropertyField( tensionColorNone, new GUIContent( "Tension None", "The Color of the Tension with no input." ) );
					EditorGUILayout.PropertyField( tensionColorFull, new GUIContent( "Tension Full", "The Color of the Tension when there is input." ) );
					EditorGUILayout.PropertyField( tensionFadeInDuration, new GUIContent( "Fade In Duration", "Time is seconds for the tension to fade in, with 0 being instant." ) );
					EditorGUILayout.PropertyField( tensionFadeOutDuration, new GUIContent( "Fade Out Duration", "Time is seconds for the tension to fade out, with 0 being instant." ) );
				}
				if( EditorGUI.EndChangeCheck() )
				{
					if( tensionFadeInDuration.floatValue < 0 )
						tensionFadeInDuration.floatValue = 0;
					if( tensionFadeOutDuration.floatValue < 0 )
						tensionFadeOutDuration.floatValue = 0;

					serializedObject.ApplyModifiedProperties();
					if( targ.tensionAccent != null )
					{
						targ.tensionAccent.color = targ.tensionColorNone;
						EditorUtility.SetDirty( targ.tensionAccent );
					}
				}
				if( targ.tensionAccent == null )
					EditorGUILayout.HelpBox( "The Tension Accent image has not been assigned. Please make sure to assign this immediately.", MessageType.Error );

				EditorGUILayout.Space();
			}

			EditorGUI.indentLevel = 0;
			// -------------------------< END TENSION >------------------------- //

			// USE ANIMATION OPTIONS //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( useAnimation );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				SetAnimation();
			}
			if( targ.useAnimation )
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField( buttonAnimator, new GUIContent( "Animator" ) );
				EditorGUI.indentLevel--;
			}

			// USE FADE OPTIONS //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( useFade );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();

				if( !targ.GetComponent<CanvasGroup>() )
					targ.gameObject.AddComponent<CanvasGroup>();

				if( targ.useFade == true )
					targ.gameObject.GetComponent<CanvasGroup>().alpha = targ.fadeUntouched;
				else
					targ.gameObject.GetComponent<CanvasGroup>().alpha = 1.0f;
			}
			if( targ.useFade )
			{
				EditorGUI.indentLevel = 1;
				{
					EditorGUI.BeginChangeCheck();
					{
						EditorGUILayout.Slider( fadeUntouched, 0.0f, 1.0f, new GUIContent( "Fade Untouched", "This controls the alpha of the button when it is NOT receiving any input." ) );
						EditorGUILayout.Slider( fadeTouched, 0.0f, 1.0f, new GUIContent( "Fade Touched", "This controls the alpha of the button when it is receiving input." ) );
					}
					if( EditorGUI.EndChangeCheck() )
					{
						serializedObject.ApplyModifiedProperties();

						if( !targ.GetComponent<CanvasGroup>() )
							targ.gameObject.AddComponent<CanvasGroup>();

						if( targ.useFade == true )
							targ.gameObject.GetComponent<CanvasGroup>().alpha = targ.fadeUntouched;
						else
							targ.gameObject.GetComponent<CanvasGroup>().alpha = 1.0f;
					}
					EditorGUI.BeginChangeCheck();
					{
						EditorGUILayout.PropertyField( fadeInDuration, new GUIContent( "Fade In Duration", "Time is seconds for the button to fade in, with 0 being instant." ) );
						EditorGUILayout.PropertyField( fadeOutDuration, new GUIContent( "Fade Out Duration", "Time is seconds for the button to fade out, with 0 being instant." ) );
					}
					if( EditorGUI.EndChangeCheck() )
					{
						if( fadeInDuration.floatValue < 0 )
							fadeInDuration.floatValue = 0;
						if( fadeOutDuration.floatValue < 0 )
							fadeOutDuration.floatValue = 0;

						serializedObject.ApplyModifiedProperties();
					}
				}
				EditorGUI.indentLevel = 0;
				EditorGUILayout.Space();
			}
		}
		#endregion

		EditorGUILayout.Space();
		
		#region SCRIPT REFERENCE
		DisplayHeader( "Script Reference", "UUI_ScriptReference" );
		if( EditorPrefs.GetBool( "UUI_ScriptReference" ) )
		{
			EditorGUILayout.Space();
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( buttonName );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();

			if( targ.buttonName == string.Empty )
				EditorGUILayout.HelpBox( "Please assign a Button Name in order to be able to get this button's input data.", MessageType.Warning );
			else
			{
				EditorGUILayout.BeginVertical( "Box" );
				GUILayout.Space( 1 );
				EditorGUILayout.LabelField( "Example Code Generator", EditorStyles.boldLabel );

				exampleCodeIndex = EditorGUILayout.Popup( "Function", exampleCodeIndex, exampleCodeOptions.ToArray() );

				EditorGUILayout.LabelField( "Function Description", EditorStyles.boldLabel );
				GUIStyle wordWrappedLabel = new GUIStyle( GUI.skin.label ) { wordWrap = true };
				EditorGUILayout.LabelField( exampleCodes[ exampleCodeIndex ].optionDescription, wordWrappedLabel );

				if( exampleCodeIndex == 3 && targ.tapCountOption == UltimateButton.TapCountOption.NoCount )
					EditorGUILayout.HelpBox( "Tap Count is not being used. Please set the Tap Count Option in order to use this option.", MessageType.Warning );

				EditorGUILayout.LabelField( "Example Code", EditorStyles.boldLabel );
				GUIStyle wordWrappedTextArea = new GUIStyle( GUI.skin.textArea ) { wordWrap = true };
				EditorGUILayout.TextArea( string.Format( exampleCodes[ exampleCodeIndex ].basicCode, buttonName.stringValue ), wordWrappedTextArea );

				GUILayout.Space( 1 );
				EditorGUILayout.EndVertical();
			}

			if( GUILayout.Button( "Open Documentation" ) )
				UltimateButtonReadmeEditor.OpenReadmeDocumentation();
		}
		#endregion

		EditorGUILayout.Space();

		#region BUTTON EVENTS
		DisplayHeader( "Button Events", "UUI_ExtraOption_01" );
		if( EditorPrefs.GetBool( "UUI_ExtraOption_01" ) )
		{
			EditorGUILayout.Space();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( onButtonDown );
			EditorGUILayout.PropertyField( onButtonUp );

			if( targ.tapCountOption != UltimateButton.TapCountOption.NoCount )
				EditorGUILayout.PropertyField( tapCountEvent );

			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
		}
		#endregion

		EditorGUILayout.Space();

		Repaint();
	}
	
	void StoreReferences ()
	{
		targ = ( UltimateButton )target;

		/* -----< ASSIGNED VARIABLES >----- */
		sizeFolder = serializedObject.FindProperty( "sizeFolder" );
		buttonBase = serializedObject.FindProperty( "buttonBase" );
		buttonHighlight = serializedObject.FindProperty( "buttonHighlight" );
		tensionAccent = serializedObject.FindProperty( "tensionAccent" );
		buttonAnimator = serializedObject.FindProperty( "buttonAnimator" );
		/* ---< END ASSIGNED VARIABLES >--- */

		/* -----< SIZE AND PLACEMENT >----- */
		positioning = serializedObject.FindProperty( "positioning" );
		relativeTransform = serializedObject.FindProperty( "relativeTransform" );
		scalingAxis = serializedObject.FindProperty( "scalingAxis" );
		anchor = serializedObject.FindProperty( "anchor" );
		touchSize = serializedObject.FindProperty( "touchSize" );
		buttonSize = serializedObject.FindProperty( "buttonSize" );
		customSpacing_X = serializedObject.FindProperty( "customSpacing_X" );
		customSpacing_Y = serializedObject.FindProperty( "customSpacing_Y" );
		/* ---< END SIZE AND PLACEMENT >--- */

		/* -----< STYLES AND OPTIONS >----- */
		imageStyle = serializedObject.FindProperty( "imageStyle" );
		inputRange = serializedObject.FindProperty( "inputRange" );
		trackInput = serializedObject.FindProperty( "trackInput" );
		transmitInput = serializedObject.FindProperty( "transmitInput" );
		receiver = serializedObject.FindProperty( "receiver" );
		tapCountOption = serializedObject.FindProperty( "tapCountOption" );
		tapCountDuration = serializedObject.FindProperty( "tapCountDuration" );
		targetTapCount = serializedObject.FindProperty( "targetTapCount" );

		baseColor = serializedObject.FindProperty( "baseColor" );
		showHighlight = serializedObject.FindProperty( "showHighlight" );
		highlightColor = serializedObject.FindProperty( "highlightColor" );
		showTension = serializedObject.FindProperty( "showTension" );
		tensionColorNone = serializedObject.FindProperty( "tensionColorNone" );
		tensionColorFull = serializedObject.FindProperty( "tensionColorFull" );
		tensionFadeInDuration = serializedObject.FindProperty( "tensionFadeInDuration" );
		tensionFadeOutDuration = serializedObject.FindProperty( "tensionFadeOutDuration" );
		useAnimation = serializedObject.FindProperty( "useAnimation" );
		useFade = serializedObject.FindProperty( "useFade" );
		fadeUntouched = serializedObject.FindProperty( "fadeUntouched" );
		fadeTouched = serializedObject.FindProperty( "fadeTouched" );
		fadeInDuration = serializedObject.FindProperty( "fadeInDuration" );
		fadeOutDuration = serializedObject.FindProperty( "fadeOutDuration" );
		/* ---< END STYLES AND OPTIONS >--- */
		
		/* ------< SCRIPT REFERENCE >------ */
		buttonName = serializedObject.FindProperty( "buttonName" );
		/* ----< END SCRIPT REFERENCE >---- */

		exampleCodeOptions = new List<string>();

		for( int i = 0; i < exampleCodes.Length; i++ )
			exampleCodeOptions.Add( exampleCodes[ i ].optionName );

		/* ------< BUTTON EVENTS >------ */
		onButtonDown = serializedObject.FindProperty( "onButtonDown" );
		onButtonUp = serializedObject.FindProperty( "onButtonUp" );
		tapCountEvent = serializedObject.FindProperty( "tapCountEvent" );
		/* ----< END BUTTON EVENTS >---- */
		
		SetHighlight();
		SetTensionAccent();
		SetAnimation();

		if( !targ.GetComponent<CanvasGroup>() )
			targ.gameObject.AddComponent<CanvasGroup>();
		if( targ.useFade == true )
			targ.gameObject.GetComponent<CanvasGroup>().alpha = targ.fadeUntouched;
		else
			targ.gameObject.GetComponent<CanvasGroup>().alpha = 1.0f;
	}

	void SetHighlight ()
	{
		if( targ.showHighlight == true && targ.buttonHighlight != null )
		{
			if( targ.buttonHighlight.gameObject.activeInHierarchy == false )
				targ.buttonHighlight.gameObject.SetActive( true );
			
			targ.buttonHighlight.color = targ.highlightColor;
		}
		else
		{
			if( targ.buttonHighlight != null && targ.buttonHighlight.gameObject.activeInHierarchy == true )
				targ.buttonHighlight.gameObject.SetActive( false );
		}
	}
	
	void SetTensionAccent ()
	{
		if( targ.showTension == true )
		{
			if( targ.tensionAccent == null )
				return;
			
			if( targ.tensionAccent != null && targ.tensionAccent.gameObject.activeInHierarchy == false )
				targ.tensionAccent.gameObject.SetActive( true );

			targ.tensionAccent.color = targ.tensionColorNone;
		}
		else
		{
			if( targ.tensionAccent != null && targ.tensionAccent.gameObject.activeInHierarchy == true )
				targ.tensionAccent.gameObject.SetActive( false );
		}
	}

	void SetAnimation ()
	{
		if( targ.useAnimation == true )
		{
			if( targ.buttonAnimator != null )
				if( targ.buttonAnimator.enabled == false )
					targ.buttonAnimator.enabled = true;
		}
		else
		{
			if( targ.buttonAnimator != null )
				if( targ.buttonAnimator.enabled == true )
					targ.buttonAnimator.enabled = false;
		}
	}

	void OnSceneGUI ()
	{
		if( Selection.activeGameObject == null )
			return;

		RectTransform trans = Selection.activeGameObject.transform.GetComponent<RectTransform>();
		Vector3 center = ( Vector2 )trans.position + ( trans.sizeDelta / 2 );

		Color normalColor = Handles.color;

		if( targ.positioning != UltimateButton.Positioning.Disabled && inputRangeHighlighted > 0 )
		{
			inputRangeHighlighted--;
			Handles.color = Color.cyan;
			Handles.DrawWireDisc( center, new Vector3( 0, 0, 1 ), ( ( targ.GetComponent<RectTransform>().sizeDelta.x ) / 2 ) * targ.inputRange );
		}

		if( targ.positioning == UltimateButton.Positioning.RelativeToTransform && targ.relativeTransform != null )
		{
			Vector3 relativeCenter = ( Vector2 )targ.relativeTransform.position + ( targ.relativeTransform.sizeDelta / 2 );
			Handles.color = Color.cyan;

			DrawWireCube( relativeCenter, targ.relativeTransform.sizeDelta * targ.relativeSpaceMod );
			Handles.Label( relativeCenter - new Vector3( ( targ.relativeTransform.sizeDelta.x / 2 ) - 5, -targ.relativeTransform.sizeDelta.y / 2, 0 ), "Relative Transform" );
		}
		
		Handles.color = normalColor;

		SceneView.RepaintAll();
	}

	void DrawWireCube ( Vector3 center, Vector2 size )
	{
		Vector3 topLeft = center + new Vector3( -size.x / 2, size.y / 2 );
		Vector3 topRight = center + ( Vector3 )( size / 2 );
		Vector3 bottomLeft = center - ( Vector3 )( size / 2 );
		Vector3 bottomRight = center - new Vector3( -size.x / 2, size.y / 2 );

		Handles.DrawLine( topLeft, topRight );
		Handles.DrawLine( topLeft, bottomLeft );
		Handles.DrawLine( bottomLeft, bottomRight );
		Handles.DrawLine( topRight, bottomRight );
	}
}

/* Written by Kaz Crowe */
/* UltimateButtonCreationEditor.cs */
public class UltimateButtonCreator
{
	public static void CreateNewUltimateButton ( GameObject buttonPrefab )
	{
		GameObject prefab = ( GameObject )Object.Instantiate( buttonPrefab, Vector3.zero, Quaternion.identity );
		prefab.name = buttonPrefab.name;
		Selection.activeGameObject = prefab;
		RequestCanvas( prefab );
	}
	
	private static void CreateNewUI ( Object buttonPrefab )
	{
		GameObject prefab = ( GameObject )Object.Instantiate( buttonPrefab, Vector3.zero, Quaternion.identity );
		prefab.name = buttonPrefab.name;
		Selection.activeGameObject = prefab;
		RequestCanvas( prefab );
	}
	
	public static void CreateNewCanvas ( GameObject button )
	{
		GameObject root = new GameObject( "Ultimate UI Canvas" );
		root.layer = LayerMask.NameToLayer( "UI" );
		Canvas canvas = root.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		root.AddComponent<GraphicRaycaster>();
		Undo.RegisterCreatedObjectUndo( root, "Create " + root.name );
		button.transform.SetParent( root.transform, false );
		CreateEventSystem();
	}
	
	private static void CreateEventSystem ()
	{
		Object esys = Object.FindObjectOfType<EventSystem>();
		if( esys == null )
		{
			GameObject eventSystem = new GameObject( "EventSystem" );
			esys = eventSystem.AddComponent<EventSystem>();
			eventSystem.AddComponent<StandaloneInputModule>();
			#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			eventSystem.AddComponent<TouchInputModule>();// If you are using a version of Unity that is 5.3.0 or higher, then remove this line.
			#endif

			Undo.RegisterCreatedObjectUndo( eventSystem, "Create " + eventSystem.name );
		}
	}

	/* PUBLIC STATIC FUNCTIONS */
	public static void RequestCanvas ( GameObject child )
	{
		Canvas[] allCanvas = Object.FindObjectsOfType( typeof( Canvas ) ) as Canvas[];

		for( int i = 0; i < allCanvas.Length; i++ )
		{
			if( allCanvas[ i ].renderMode == RenderMode.ScreenSpaceOverlay && allCanvas[ i ].enabled == true && ValidateCanvasScalerComponent( allCanvas[ i ] ) )
			{
				child.transform.SetParent( allCanvas[ i ].transform, false );
				CreateEventSystem();
				return;
			}
		}
		CreateNewCanvas( child );
	}

	static bool ValidateCanvasScalerComponent ( Canvas canvas )
	{
		if( !canvas.GetComponent<CanvasScaler>() )
			return true;
		else if( canvas.GetComponent<CanvasScaler>().uiScaleMode == CanvasScaler.ScaleMode.ConstantPixelSize )
			return true;

		return false;
	}
}