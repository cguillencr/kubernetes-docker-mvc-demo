# kubernetes Docker and .Net(core) demo


Demo it's a core project which returns a json response in a basic REST GET controller. This application will be deployed using dokcer to generate the image and kubernetes to release it.


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