# Kubernetes Volume Add-on
This add-on provisions Kubernetes volumes for use by Kubernetes workloads.

## Prerequisites
* Version 9.1 of Apprenda or greater.
* A storage class configured for your Kubernetes cluster. For example:

        apiVersion: storage.k8s.io/v1beta1
        kind: StorageClass
        metadata:
          name: glusterfs-storage
        provisioner: kubernetes.io/glusterfs
        parameters:
          resturl: "http://10.1.7.83:8080"

## Installation
*If using basic auth for your Kubernetes API server, skip step 1. The compiled add-on can be found under [releases](https://github.com/apprenda/Kubernetes-Volume-Addon/releases).*

1. Unzip ```addon.zip```, add a Kubernetes API server client certificate (.pfx) to the archive, and re-zip the archive.
2. Use the SOC to create a new add-on with ```addon.zip```.
3. Edit the add-on configuration.
    * Edit the description to include the type of storage the add-on will provision.
    * Set ```Location``` to the URL of the Kubernetes API server (the same URL as the one used to add the Kubernetes cluster to Apprenda) that hosts the storage you would like to provide.
    * If using basic auth, set the username and password (if not, leave these fields empty).
    * Under configuration, set the namespace for Apprenda workloads (the same as that used to add the cluster to Apprenda), the storage class you would like to provision (e.g. nfs, glusterfs), and optionally the name and password of the pfx file in the archive (from step 1).
    * Example configuration:

        ![Edit General](/images/edit-general.png)

        ![Edit Configuration](/images/edit-configuration.png)

4. Test the add-on
    * This will create and delete a persistent volume claim using the specified storage class under the specified namespace.

## Usage
1. Provision a new instance of the add-on from the Apprenda developer portal.
2. Add the add-on token to your Kubernetes POD spec. For example:

        kind: Deployment
        apiVersion: extensions/v1beta1
        metadata:
          name: busybox
          namespace: acp
        spec:
          replicas: 1
          template:
            metadata:
              labels:
                app: busybox
            spec:
              containers:
              - name: busybox
                image: busybox:latest
                command:
                - tail
                - -f
                - /dev/null
                volumeMounts:
                - name: volume
                  mountPath: /mnt
              volumes:
              - name: volume
                persistentVolumeClaim:
                  claimName: $#ADDON-alias#$

3. Deploy the application and it will have access to the storage you specified.
    * Example output:

        ![Mounted Volume](/images/mounted-volume.png)
