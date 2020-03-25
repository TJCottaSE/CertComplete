echo "Building Docs"
"C:\Program Files (x86)\Doxygen\doxygen" C:\Users\tcotta.SHIFT4\source\repos\CertComplete\CertComplete\CertCompleteDoxygenConfig
echo "Creating Directories"
mkdir C:\Users\tcotta.SHIFT4\source\repos\CertComplete\CertComplete\bin\Release\Docs
mkdir C:\Users\tcotta.SHIFT4\source\repos\CertComplete\CertComplete\bin\Release\Log
mkdir C:\Users\tcotta.SHIFT4\source\repos\CertComplete\CertComplete\bin\Debug\Docs
mkdir C:\Users\tcotta.SHIFT4\source\repos\CertComplete\CertComplete\bin\Debug\Log
echo "Copying files"
robocopy C:\Users\t_cota\source\repos\CertComplete\CertComplete\html C:\Users\t_cota\source\repos\CertComplete\CertComplete\bin\Release\Docs /mir
robocopy C:\Users\t_cota\source\repos\CertComplete\CertComplete\html C:\Users\t_cota\source\repos\CertComplete\CertComplete\bin\Debug\Docs /mir
copy C:\Users\t_cota\source\repos\CertComplete\CertComplete\authCapture.jpg C:\Users\t_cota\source\repos\CertComplete\CertComplete\bin\Release\Docs\authCapture.jpg
copy C:\Users\t_cota\source\repos\CertComplete\CertComplete\authCapture.jpg C:\Users\t_cota\source\repos\CertComplete\CertComplete\bin\Debug\Docs\authCapture.jpg
copy C:\Users\t_cota\source\repos\CertComplete\CertComplete\testCreator.jpg C:\Users\t_cota\source\repos\CertComplete\CertComplete\bin\Release\Docs\testCreator.jpg
copy C:\Users\t_cota\source\repos\CertComplete\CertComplete\testCreator.jpg C:\Users\t_cota\source\repos\CertComplete\CertComplete\bin\Debug\Docs\testCreator.jpg
copy C:\Users\t_cota\source\repos\CertComplete\CertComplete\testRun.jpg C:\Users\t_cota\source\repos\CertComplete\CertComplete\bin\Release\Docs\testRun.jpg
copy C:\Users\t_cota\source\repos\CertComplete\CertComplete\testRun.jpg C:\Users\t_cota\source\repos\CertComplete\CertComplete\bin\Debug\Docs\testRun.jpg
copy C:\Users\t_cota\source\repos\CertComplete\CertComplete\paxConfig.jpg C:\Users\t_cota\source\repos\CertComplete\CertComplete\bin\Release\Docs\paxConfig.jpg
copy C:\Users\t_cota\source\repos\CertComplete\CertComplete\paxConfig.jpg C:\Users\t_cota\source\repos\CertComplete\CertComplete\bin\Debug\Docs\paxConfig.jpg
copy C:\Users\t_cota\source\repos\CertComplete\CertComplete\json.jpg C:\Users\t_cota\source\repos\CertComplete\CertComplete\bin\Release\Docs\json.jpg
copy C:\Users\t_cota\source\repos\CertComplete\CertComplete\json.jpg C:\Users\t_cota\source\repos\CertComplete\CertComplete\bin\Debug\Docs\json.jpg
copy C:\Users\t_cota\source\repos\CertComplete\CertComplete\innowiConfig.jpg C:\Users\t_cota\source\repos\CertComplete\CertComplete\bin\Release\Docs\innowiConfig.jpg
copy C:\Users\t_cota\source\repos\CertComplete\CertComplete\innowiConfig.jpg C:\Users\t_cota\source\repos\CertComplete\CertComplete\bin\Debug\Docs\innowiConfig.jpg
echo "Complete"
exit 0
