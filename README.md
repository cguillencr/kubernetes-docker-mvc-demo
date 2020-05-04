# kubernetes Docker and .Net(core) demo

This demo have two applications. There is an **UI** app which reach REST controller in the **Core** app, so far they has just one service that returns an array of strings. **Core** it's a statefull app thats why has a thread sleep in the Startup process to simulate the application is recovering state. 
These applications will be deployed using dokcer to generate the image and kubernetes to release it. **UI** will handle the users request that come over "internet" meanwhile **Core** will be an internal server who provide data only to the UI.


## Kubernetes dashboard
First install Kubernetes dashboard to see the server installation easier

```
#https://kubernetes.io/docs/tasks/access-application-cluster/web-ui-dashboard/
kubectl apply -f https://raw.githubusercontent.com/kubernetes/dashboard/v2.0.0/aio/deploy/recommended.yaml
kubectl proxy

# Allow Dashboard external request 
# https://www.thegeekdiary.com/how-to-access-kubernetes-dashboard-externally/
kubectl  edit service kubernetes-dashboard -n kubernetes-dashboard

#Change type: NodePort                   ### clusterIP to NodePort
kubectl -n kube-system get services

# Get port mapping to 443 by NodePort
kubectl get services -n kubernetes-dashboard

#Execute this to get a token 
kubectl -n kube-system describe $(kubectl -n kube-system get secret -n kube-system -o name | grep namespace) | grep token:

# Navigate to https://<server ip>:<port>/#/login
```


## Get demo in to the server

The folllowing step install both applications.

1. Execute the following command in the server
```
git clone https://github.com/cguillencr/kubernetes-docker-mvc-demo.git
cd kubernetes-docker-mvc-demo/Demo/
```

2.  Create a locally docker images
```
docker build -t "cguillenmendez/core:0.0.1" .
cd ../UI/
docker build -t "cguillenmendez/ui:0.0.2" .
docker images
```

Validate "cguillenmendez/core" and  "cguillenmendez/ui" images was created.

2.1 Update the image to docker.io
```
docker push cguillenmendez/core:0.0.1
docker push cguillenmendez/ui:0.0.2
```

2.2. Test application locally using just dokcer. (Not required)

```
docker run -d -p 1000:80 --name core cguillenmendez/core:0.0.1
docker run -d -p 1001:80 --name ui cguillenmendez/ui:0.0.2
docker ps -a
```
Navigate to http://<ip>:1000/api/values and validate this output
```
["value1","value2"]
```

3. Create a K8 deployment
```
 kubectl create deployment core --image=cguillenmendez/core:0.0.1
 kubectl delete -n default deployment ui
 kubectl create deployment ui --image=cguillenmendez/ui:0.0.2
 ```

4. Create a service to expose the last pod
The UI it's exposed as **NodePort** in order to reach the server externally mean while the core app it's exposed as **ClusterIp** and only pod inside the Cluster can reach it.

```
kubectl get services
kubectl expose deployment core --port 80
kubectl expose deployment ui --type="NodePort" --port 80
kubectl get services
```

Navigate to http://<ip>:<port in lastt command>/page1 and validate this output
```
["value1","value2"]
```


### Test #1 - Simulate the API it down for any reason.

The following test simulates the API service, which provide data it's down. The first test, the pod is down but service is up and running but in the second test, the pod and the service are down as well. The idea it's to know how the UI behaves.

After all the cloud it's configured  get the default page and then stop the API pod
```
curl --location --request GET 'http://localhost:32391/page1'
kubectl delete -n default deployment core
```
The kubernates bashboard shows how the: Deployment, Pod and Replica Set were deleted. but the service its still up.  Retrieve the page again and check the logs. Be aware the pod name ~~ui-766c879b4b-hnp5m~~ will be different it your enviroment.

```
curl --location --request GET 'http://localhost:32391/page1'
kubectl exec -it ui-766c879b4b-hnp5m -- cat /tmp/demo/out.log
```
This is the output
```
2020-05-03 23:25:47,744 | DEBUG | API error. Status: 0 |  Content:  |  ErrorMessage: The operation was canceled. |  ErrorMessage: System.OperationCanceledException: The operation was canceled.
   at System.Net.HttpWebRequest.GetResponse()
   at RestSharp.Http.<ExecuteRequest>g__GetRawResponse|181_1(WebRequest request)
   at RestSharp.Http.ExecuteRequest(String httpMethod, Action`1 prepareRequest)
```

Then for the second scenario delete the service, retrieve the page again and check the logs.  Be aware the pod name ~~ui-766c879b4b-hnp5m~~ will be different it your enviroment.

```
kubectl delete -n default service core
curl --location --request GET 'http://localhost:32391/page1'
kubectl exec -it ui-766c879b4b-hnp5m -- cat /tmp/demo/out.log
```
This is the output
```
2020-05-03 23:26:28,100 | DEBUG | API error. Status: 0 |  Content:  |  ErrorMessage: No such device or address No such device or address |  ErrorMessage: System.Net.WebException: No such device or address No such device or address ---> System.Net.Http.HttpRequestException: No such device or address ---> System.Net.Sockets.SocketException: No such device or address
   at System.Net.Http.ConnectHelper.ConnectAsync(String host, Int32 port, CancellationToken cancellationToken)
   --- End of inner exception stack trace ---
   at System.Net.Http.ConnectHelper.ConnectAsync(String host, Int32 port, CancellationToken cancellationToken)
   at System.Threading.Tasks.ValueTask`1.get_Result()
   at System.Net.Http.HttpConnectionPool.CreateConnectionAsync(HttpRequestMessage request, CancellationToken cancellationToken)
   at System.Threading.Tasks.ValueTask`1.get_Result()
   at System.Net.Http.HttpConnectionPool.WaitForCreatedConnectionAsync(ValueTask`1 creationTask)
   at System.Threading.Tasks.ValueTask`1.get_Result()
   at System.Net.Http.HttpConnectionPool.SendWithRetryAsync(HttpRequestMessage request, Boolean doRequestAuth, CancellationToken cancellationToken)
   at System.Net.Http.RedirectHandler.SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
   at System.Net.Http.DecompressionHandler.SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
   at System.Net.Http.HttpClient.FinishSendAsyncUnbuffered(Task`1 sendTask, HttpRequestMessage request, CancellationTokenSource cts, Boolean disposeCts)
   at System.Net.HttpWebRequest.SendRequest()
   at System.Net.HttpWebRequest.GetResponse()
   --- End of inner exception stack trace ---
   at System.Net.HttpWebRequest.GetResponse()
   at RestSharp.Http.<ExecuteRequest>g__GetRawResponse|181_1(WebRequest request)
   at RestSharp.Http.ExecuteRequest(String httpMethod, Action`1 prepareRequest)
```
#### Test #1 - Conclution
If the API its down the UI will get a statud 0 response but the error details change that let developers to  manage handle the error if it's required.
