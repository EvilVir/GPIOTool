# GPIOTool
Simple Mono application for fiddling with RPi's GPIO pins. Runs as CLI application, that gives you simple list of all GPIO pins on your board and then you can change individual pin's properties (state, mode/direction etc.), pool for it's state (in read mode), tag them and more. Serves good for simple fiddling/prototyping with Raspberry.

Please note that this app not polished and can have some stability issues - it's just development/prototyping helper tool, not industrial grade software ;)

Uses [WiringPi.NET](https://github.com/EvilVir/WiringPi.NET) library, which is referenced as GIT submodule in this repository.

