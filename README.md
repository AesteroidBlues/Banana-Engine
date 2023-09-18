# Banana Engine
![Yes this joke is why it's called Banana Engine](doc/thejoke.jpeg)

## Features
- 2D Animation with animation state machines
- Integrated [LiteDB](https://github.com/mbdavid/LiteDB) serialization/deserialization
- Gracefully handle runtime issues using Debug.Assert
- Integrated ImGui
- Scene graph

## Troubleshooting
### I crash on startup because cimgui.dylib/.so/.dll is not found!
Copy the appropriate cimgui library for your system from the /lib/ folder to the output directory of the sample

### You didn't ship a library that's compatible with my architecture!
Sorry pal, you'll have to [compile the native dependencies yourself](https://github.com/AesteroidBlues/startup-hell-native-dependencies)

### I need a spritefont!
I've included UbuntuMono-R as well as the .spritefont file you'll need as well as the built .xnb in res/bin. The rationale here is it's currently not possible to build spritefonts on any platform other than Windows but other platforms will use the built xnb just fine.

## Notes
I had trouble getting intellisense working on the latest VS Code C# due to moving the obj/ folder in Directory.Build.Props. v1.26.0 seems to work.