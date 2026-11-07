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

The generated prototype contains a desert ground plane, perimeter walls, player capsule with CharacterController movement, basic touch directional buttons, a follow camera, six AI guards, a hostage, objective trigger zones, and an Arabic mission HUD.

## Mission flow

- Enter the base entry zone.
- Reach the hostage and use the on-screen Arabic rescue button (or keyboard E in the Editor).
- Reach the extraction zone to complete the mission and receive the prototype reward message.

## Combat controls

- **منظار** toggles a simple camera field-of-view zoom for mobile sniper aiming.
- **إلغاء المنظار** returns to the normal camera field of view.
- **إطلاق** fires a center-screen hitscan ray; placeholder guards can be eliminated with one hit.

The scope is a basic zoom mode, not a finished optical-scope rendering or ballistic simulation.

## Important limitations

- The scene generator runs inside the Unity Editor; it does not run on GitHub or directly on an Android tablet.
- Guard AI is an early prototype: guards patrol, detect the player within a forward vision cone, chase, and attack at close range. Walls and obstacles can block vision.
- Nearby guards investigate the player's position when the sniper is fired; this is a simple sound-alert simulation, not a full stealth sound system.
- Player health starts at 100 and decreases when a guard reaches attack range. There is not yet a death/restart flow or healing system.
- Mission 01 completion and the 1200-credit reward are saved locally with PlayerPrefs.
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
