# Odd–even sort

Implementation of the parallel *odd–even sort* algorithm using MPI.NET

## Dependencies

- [.NET 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- [Microsoft-MPI](https://learn.microsoft.com/en-us/message-passing-interface/microsoft-mpi)

### Install Microsoft MPI v10.1.1

```powershell
.\script\install_mpi.ps1
```

## Build and Run Locally

### Build project

```powershell
.\script\build.ps1
```

### Run with MPI

```powershell
mpiexec.exe -n 4 .\Sort\bin\Debug\net7.0\Sort.exe $path_to_input_file $path_to_output_file
```

#### Input file format

A text file consisting of a single line containing decimal numbers separated by spaces

#### Output file format

A text file consisting of a single line containing sorted decimal numbers separated by spaces

## Run tests

```powershell
.\script\test.ps1
```