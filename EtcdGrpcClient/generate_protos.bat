..\packages\Grpc.Tools.1.6.0\tools\windows_x64\protoc.exe -I./protos --csharp_out generated --grpc_out generated ./protos/rpc.proto --plugin=protoc-gen-grpc=../packages/Grpc.Tools.1.6.0/tools/windows_x64/grpc_csharp_plugin.exe

..\packages\Grpc.Tools.1.6.0\tools\windows_x64\protoc.exe -I./protos --csharp_out generated --grpc_out generated ./protos/kv.proto --plugin=protoc-gen-grpc=../packages/Grpc.Tools.1.6.0/tools/windows_x64/grpc_csharp_plugin.exe

..\packages\Grpc.Tools.1.6.0\tools\windows_x64\protoc.exe -I./protos --csharp_out generated --grpc_out generated ./protos/auth.proto --plugin=protoc-gen-grpc=../packages/Grpc.Tools.1.6.0/tools/windows_x64/grpc_csharp_plugin.exe