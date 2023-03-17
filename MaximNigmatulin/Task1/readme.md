# Launch

On linux:
```sh
export MPJ_HOME=<path_to_mpj>
./gradlew run -PnumProcesses="<np | np=2^k>" -PinputFilename="<input_file>" -PoutputFilename="<output_file>" -PnumLen="<input_len>"
```

for example:
```sh
./gradlew run -PnumProcesses="16" -PinputFilename="./resources/1Mil.txt" -PoutputFilename="./out.txt" -PnumLen="1000000"
```


## Possible problems

env var might be cached by gradle daemon, it can be stopped with
```sh
./gradlew --stop
```
