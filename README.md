# Desert Quiet

A mobile third-person desert stealth prototype built with Unity 2022.3 LTS and URP.

## Current repository status

The repository contains the initial Unity project structure, runtime prototype scripts, and an Editor scene generator for Mission 01. This is a development prototype, not a finished game or a tested APK.

## Generate Mission 01

1. Open the repository as a Unity project using Unity 2022.3 LTS.
2. Wait for Package Manager to resolve the Universal Render Pipeline dependency.
3. In the Unity Editor menu, choose **Desert Quiet > Build Mission 01 Scene**.
4. The tool creates and saves the scene at Assets/Scenes/Mission01_DesertBase.unity.
5. Open that scene and press Play.

The generated prototype contains a desert ground plane, perimeter walls, player capsule with CharacterController movement, basic touch directional buttons, a follow camera, six placeholder patrol guards, a hostage, objective trigger zones, and an Arabic mission HUD.

## Mission flow

- Enter the base entry zone.
- Reach the hostage and press E to rescue.
- Reach the extraction zone to complete the mission and receive the prototype reward message.

## Important limitations

- The scene generator runs inside the Unity Editor; it does not run on GitHub.
- Guard patrols are simple placeholder movement, not finished stealth/combat AI.
- The hostage interaction currently uses keyboard E. A dedicated mobile interaction button is still needed.
- Visuals use Unity primitive meshes and placeholder materials.
- The project has not yet been opened or compiled in Unity in this workflow.
- No Android APK has been produced or tested.

## Android build

After the project is opened and the generated scene is verified in Unity, configure Android as the Build Automation target. Android player settings, package identifier, orientation, and signing configuration must be checked before producing an installable APK.

## Repository layout

- Assets/Scripts: runtime gameplay scripts
- Assets/Editor: Unity Editor scene generation tools
- Assets/Scenes: generated Unity scenes
- Packages/manifest.json: package dependencies
- ProjectSettings/ProjectVersion.txt: Unity editor version
