# kubernetes Docker and .Net(core) demo

This demo have two applications. There is an **UI** app which reach REST controller in the **API** app, so far they has just one service that returns an array of strings. **API** it's a statefull app thats why has a thread sleep in the Startup process to simulate the application is recovering state. 
These applications will be deployed using dokcer to generate the image and kubernetes to release it. **UI** will handle the users request that come over "internet" meanwhile **API** will be an internal server who provide data only to the UI.

There are two additionals applicaitions **APIv2**  and **APIv3**. the first one it to simulated and api upgrade meanwhile users create some traffic over the **UI** . The third one has the same purpose but it daoesn't required time to boots up. 
In terms of docker **API** will ne the version 0.0.*, **APIv2** 0.1.* and **APIv3** 0.2.*
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
```

2.  Create a locally docker images
```
cd /root/kubernetes-docker-mvc-demo/API/
docker build -t "cguillenmendez/core:0.0.2" .
cd /root/kubernetes-docker-mvc-demo/APIv2/
docker build -t "cguillenmendez/core:0.1.0" .
cd /root/kubernetes-docker-mvc-demo/APIv3/
docker build -t "cguillenmendez/core:0.2.0" .
cd /root/kubernetes-docker-mvc-demo/UI/
docker build -t "cguillenmendez/ui:0.0.3" .
docker images
```

Validate "cguillenmendez/core 0.0.2", "cguillenmendez/core 0.1.0", "cguillenmendez/core 0.2.0" a and  "cguillenmendez/ui" 0.0.3 images was created.

2.1 Update the image to docker.io
```
docker push cguillenmendez/core:0.0.2
docker push cguillenmendez/core:0.1.0
docker push cguillenmendez/core:0.2.0
docker push cguillenmendez/ui:0.0.3
```


3. Create a K8 deployment
```
 kubectl create deployment core --image=cguillenmendez/core:0.0.2
 kubectl create deployment core --image=cguillenmendez/core:0.1.0
 kubectl create deployment core --image=cguillenmendez/core:0.2.0
 kubectl delete -n default deployment ui
 kubectl create deployment ui --image=cguillenmendez/ui:0.0.3
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
kubectl exec -it ui-7ddbdc65-q95mj -- cat /tmp/ui/out.log
```
This is the output
```
2020-05-03 23:25:47,744 | DEBUG | API error. Status: 0 |  Content:  |  ErrorMessage: The operation was canceled. |  ErrorMessage: System.OperationCanceledException: The operation was canceled.
   at System.Net.HttpWebRequest.GetResponse()
   at RestSharp.Http.<ExecuteRequest>g__GetRawResponse|181_1(WebRequest request)
   at RestSharp.Http.ExecuteRequest(String httpMethod, Action`1 prepareRequest)
```

Then for the second scenario delete the service, retrieve the page again and check the logs.  Be aware the pod name ~~ui-7ddbdc65-q95mj~~ will be different it your enviroment.

```
kubectl delete -n default service core
curl --location --request GET 'http://localhost:32391/page1'
kubectl exec -it ui-7ddbdc65-q95mj -- cat /tmp/ui/out.log
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


### Test #2 - Upgrade API image meanwhile the User are retriaving information.

This scenario simulates an API upgrade from version 0.0.2 to 0.1.0 meanwhile there is an user navigating through the **UI** 

```
kubectl get deployments
kubectl set image deployment/core core=cguillenmendez/core:0.1.0 --record
curl --location --request GET 'http://localhost:32391/page1'
```
The last curl try to get the page but the pod needs 20 secs to start up thats why to following error is in the log

```
2020-05-04 02:47:39,538 | 35 | DEBUG | API error. Status: 0 |  Content:  |  ErrorMessage: The operation has timed out. |  ErrorMessage: System.Net.WebException: The operation has timed out.
   at System.Net.HttpWebRequest.GetResponse()
   at RestSharp.Http.<ExecuteRequest>g__GetRawResponse|181_1(WebRequest request)
   at RestSharp.Http.ExecuteRequest(String httpMethod, Action`1 prepareRequest)
2020-05-04 02:47:39,538 | 35 | DEBUG | Ending processing request /page1
```
#### Test #2 - Conclution
Even if the service it's up the UI will get timeouts when pod behind it's down.

kubectl rollout status deployment.v1.apps/core

