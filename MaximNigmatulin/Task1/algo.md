# Parallel Quicksort Algorithm

1. Randomly choose pivot from one of the processes and broadcast it to every process
2. Each process divides its list using partition algorithm
3. Each process in the "upper half" of the processes sends its "low list" to a partner process in the lower half of the process list and receives a "high list" in return. Partner process can be chosen how we want
4. Processes divide themselves into two groups, each group starts its own || quicksort
5. 