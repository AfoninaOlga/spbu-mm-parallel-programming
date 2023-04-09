# How to use

### Download MPJ Express

- load [MPJ Archive](https://sourceforge.net/projects/mpjexpress/files/releases/mpj-v0_44.zip/download)
- unpack it
- set ```MPJ_HOME``` env var to unpacked archive root path
- add ```<path to MPJ_HOME>/bin``` to ```PATH```

### Launch

On linux:
```sh
./gradlew run -PnumProcesses="<np | np=2^k>" -PinputFilename="<input_file_path>" -PoutputFilename="<output_file_path>"
```
where:
- ```PnumProcesses```: num of processes algorithm should be launched with, (power of 2)
- ```PinputFilename```: name of input file, containing ```n``` numbers separated by single ```space``` character
- ```PoutputFilename```: name of output file, in which ```n``` sorted files, separated by single ```space``` character, will be written after the algorithm completion

for example:
```sh
./gradlew run -PnumProcesses="8" -PinputFilename="./resources/1Mil.txt" -PoutputFilename="./out.txt"
```


## Possible problems

env var ```MPJ_HOME``` or ```PATH``` might be cached by gradle daemon, it can be stopped with
```sh
./gradlew --stop
```
