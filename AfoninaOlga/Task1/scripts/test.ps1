$exitcode = 0
.\scripts\build.ps1
Get-ChildItem -Path .\test\ -Directory |
        ForEach-Object {
            $input = Join-Path $_.FullName input.txt
            $output = Join-Path $_.FullName output.txt
            $expected = Join-Path $_.FullName expected.txt
            mpiexec.exe -n 4 .\Sort\bin\Debug\net7.0\Sort.exe $input $output
            if ($LastExitCode) {
                $exitcode = 1
            }
            $diff = Compare-Object -ReferenceObject (Get-Content $expected) -DifferenceObject (Get-Content $output) 
            if ($diff)
            {
                Write-Host ("Test Failed: ", $_.Name) -ForegroundColor Red
                $diff
                $exitcode = 1
            }
            else
            {
                Write-Host ("Test Passed: ", $_.Name) -ForegroundColor Green
                Remove-Item -Verbose -Force $output
            }
        }
exit $exitcode
