# LanderNet
Assembler+WPF scroller game

Vertical space scroller game which uses my old pascal/assembly code for graphics and .NET/WPF for everything else.
- sprite graphics is composed entirely by assembly code, in background thread
- WPF is used to display text and menu
- pseudo-3D animated objects (drawn by me in 3ds Max many years ago for DOS game; source files are lost, only 8-bit bitmaps were found)
- SlimDX is used only for sound (could not get any standard library functions to work properly with multiple simultaneous sounds)

Controls:

- arrow buttons for movement
- space to fire bullets
- left control to fire rockets


![LanderNet screenshot](http://lander-net.googlecode.com/svn/trunk/Screenshot.png)