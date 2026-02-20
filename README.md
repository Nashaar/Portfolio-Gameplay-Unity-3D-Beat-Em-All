# Gameplay-Portfolio-Unity-3D-Beat'EmAll

## Overview
Gameplay Prototype of a 3D Beat 'Em All with Walking and Lock-On Cameras.
The project is still in early development and focuses for now on camera controls and lock-on system integration with Cinemachine.

This repository contains only the core gameplay scripts for demonstration purposes.

## Technical Focus
- Rigidbody3D-based movement system (velocity control & air/ground state separation)
- Target detection using sphere overlap
- Target validation using angle and raycast filtering
- Separation between:
  - Free-look orbit camera
  - Lock-on follow camera
- Integration of gaming peripherals (mouse & keyboard, gamepad and in the future joystick)

## System Architecture Highlights
- Lock-on target selection pipeline:
  1. Sphere detection
  2. Angle filtering
  3. Distance prioritization
  4. Raycast visibility validation

## Planned integrations
- Enemy delocking due to high distance or angle with the player
- 3D animations
- AI behaviour and pathing for groups of enemies
- Types of weapon with own fighting style, damages and animations based on ScriptableObjects
- UI/UX

## Technical Issues To Solved
- Camera jitter during movement
- Selection of enemies in front of the player
- Position of walking camera when leaving the lock-on mode

## Video Demonstration
https://youtu.be/4ylEQflGLy0

## Author
Tom BAUDIN - Gameplay programming student - Unity / C#
