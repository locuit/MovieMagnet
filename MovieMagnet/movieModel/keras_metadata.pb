
Ñ1root"_tf_keras_layer*±1{
  "name": "model",
  "trainable": true,
  "expects_training_arg": false,
  "dtype": "float32",
  "batch_input_shape": null,
  "autocast": false,
  "class_name": "Functional",
  "config": {
    "name": "model",
    "layers": [
      {
        "name": "user_input",
        "class_name": "InputLayer",
        "config": {
          "sparse": false,
          "ragged": false,
          "name": "user_input",
          "dtype": "float32",
          "batch_input_shape": {
            "class_name": "TensorShape",
            "items": [
              null,
              1
            ]
          }
        },
        "inbound_nodes": []
      },
      {
        "name": "movie_input",
        "class_name": "InputLayer",
        "config": {
          "sparse": false,
          "ragged": false,
          "name": "movie_input",
          "dtype": "float32",
          "batch_input_shape": {
            "class_name": "TensorShape",
            "items": [
              null,
              1
            ]
          }
        },
        "inbound_nodes": []
      },
      {
        "name": "user_embedding",
        "class_name": "Embedding",
        "config": {},
        "inbound_nodes": [
          [
            "user_input",
            0,
            0
          ]
        ]
      },
      {
        "name": "movie_embedding",
        "class_name": "Embedding",
        "config": {},
        "inbound_nodes": [
          [
            "movie_input",
            0,
            0
          ]
        ]
      },
      {
        "name": "flatten",
        "class_name": "Flatten",
        "config": {
          "data_format": "channels_last",
          "name": null,
          "dtype": "float32",
          "trainable": true
        },
        "inbound_nodes": [
          [
            "user_embedding",
            0,
            0
          ]
        ]
      },
      {
        "name": "flatten_1",
        "class_name": "Flatten",
        "config": {
          "data_format": "channels_last",
          "name": null,
          "dtype": "float32",
          "trainable": true
        },
        "inbound_nodes": [
          [
            "movie_embedding",
            0,
            0
          ]
        ]
      },
      {
        "name": "concatenate",
        "class_name": "Concatenate",
        "config": {},
        "inbound_nodes": [
          [
            "flatten",
            0,
            0
          ],
          [
            "flatten_1",
            0,
            0
          ]
        ]
      },
      {
        "name": "dense",
        "class_name": "Dense",
        "config": {
          "units": 256,
          "activation": "relu",
          "use_bias": true,
          "kernel_initializer": {
            "class_name": "GlorotUniform",
            "config": {
              "seed": null
            }
          },
          "bias_initializer": {
            "class_name": "Zeros",
            "config": {}
          },
          "kernel_regularizer": null,
          "bias_regularizer": null,
          "kernel_constraint": null,
          "bias_constraint": null,
          "name": null,
          "dtype": "float32",
          "trainable": true
        },
        "inbound_nodes": [
          [
            "concatenate",
            0,
            0
          ]
        ]
      },
      {
        "name": "dropout",
        "class_name": "Dropout",
        "config": {
          "rate": 0.3,
          "noise_shape": null,
          "seed": null,
          "name": null,
          "dtype": "float32",
          "trainable": true
        },
        "inbound_nodes": [
          [
            "dense",
            0,
            0
          ]
        ]
      },
      {
        "name": "dense_1",
        "class_name": "Dense",
        "config": {
          "units": 128,
          "activation": "relu",
          "use_bias": true,
          "kernel_initializer": {
            "class_name": "GlorotUniform",
            "config": {
              "seed": null
            }
          },
          "bias_initializer": {
            "class_name": "Zeros",
            "config": {}
          },
          "kernel_regularizer": null,
          "bias_regularizer": null,
          "kernel_constraint": null,
          "bias_constraint": null,
          "name": null,
          "dtype": "float32",
          "trainable": true
        },
        "inbound_nodes": [
          [
            "dropout",
            0,
            0
          ]
        ]
      },
      {
        "name": "dropout_1",
        "class_name": "Dropout",
        "config": {
          "rate": 0.3,
          "noise_shape": null,
          "seed": null,
          "name": null,
          "dtype": "float32",
          "trainable": true
        },
        "inbound_nodes": [
          [
            "dense_1",
            0,
            0
          ]
        ]
      },
      {
        "name": "dense_2",
        "class_name": "Dense",
        "config": {
          "units": 1,
          "activation": "linear",
          "use_bias": true,
          "kernel_initializer": {
            "class_name": "GlorotUniform",
            "config": {
              "seed": null
            }
          },
          "bias_initializer": {
            "class_name": "Zeros",
            "config": {}
          },
          "kernel_regularizer": null,
          "bias_regularizer": null,
          "kernel_constraint": null,
          "bias_constraint": null,
          "name": null,
          "dtype": "float32",
          "trainable": true
        },
        "inbound_nodes": [
          [
            "dropout_1",
            0,
            0
          ]
        ]
      }
    ],
    "input_layers": [
      [
        "user_input",
        0,
        0
      ],
      [
        "movie_input",
        0,
        0
      ]
    ],
    "output_layers": [
      [
        "dense_2",
        0,
        0
      ]
    ]
  },
  "shared_object_id": 1,
  "build_input_shape": {
    "class_name": "TensorShape",
    "items": [
      null,
      1
    ]
  }
}2
âroot.layer-0"_tf_keras_input_layer*²{"class_name":"InputLayer","name":"user_input","dtype":"float32","sparse":false,"ragged":false,"batch_input_shape":{"class_name":"TensorShape","items":[null,1]},"config":{"sparse":false,"ragged":false,"name":"user_input","dtype":"float32","batch_input_shape":{"class_name":"TensorShape","items":[null,1]}}}2
äroot.layer-1"_tf_keras_input_layer*´{"class_name":"InputLayer","name":"movie_input","dtype":"float32","sparse":false,"ragged":false,"batch_input_shape":{"class_name":"TensorShape","items":[null,1]},"config":{"sparse":false,"ragged":false,"name":"movie_input","dtype":"float32","batch_input_shape":{"class_name":"TensorShape","items":[null,1]}}}2
’root.layer_with_weights-0"_tf_keras_layer*Û{
  "name": "user_embedding",
  "trainable": true,
  "expects_training_arg": false,
  "dtype": "float32",
  "batch_input_shape": null,
  "autocast": false,
  "class_name": "Embedding",
  "config": {},
  "shared_object_id": 2,
  "build_input_shape": {
    "class_name": "TensorShape",
    "items": [
      null,
      1
    ]
  }
}2
“root.layer_with_weights-1"_tf_keras_layer*Ü{
  "name": "movie_embedding",
  "trainable": true,
  "expects_training_arg": false,
  "dtype": "float32",
  "batch_input_shape": null,
  "autocast": false,
  "class_name": "Embedding",
  "config": {},
  "shared_object_id": 3,
  "build_input_shape": {
    "class_name": "TensorShape",
    "items": [
      null,
      1
    ]
  }
}2
ôroot.layer-4"_tf_keras_layer*Ê{
  "name": "flatten",
  "trainable": true,
  "expects_training_arg": false,
  "dtype": "float32",
  "batch_input_shape": null,
  "autocast": false,
  "class_name": "Flatten",
  "config": {
    "data_format": "channels_last",
    "name": null,
    "dtype": "float32",
    "trainable": true
  },
  "shared_object_id": 4,
  "build_input_shape": {
    "class_name": "TensorShape",
    "items": [
      null,
      1,
      128
    ]
  }
}2
öroot.layer-5"_tf_keras_layer*Ì{
  "name": "flatten_1",
  "trainable": true,
  "expects_training_arg": false,
  "dtype": "float32",
  "batch_input_shape": null,
  "autocast": false,
  "class_name": "Flatten",
  "config": {
    "data_format": "channels_last",
    "name": null,
    "dtype": "float32",
    "trainable": true
  },
  "shared_object_id": 5,
  "build_input_shape": {
    "class_name": "TensorShape",
    "items": [
      null,
      1,
      128
    ]
  }
}2
†root.layer-6"_tf_keras_layer*Ü{
  "name": "concatenate",
  "trainable": true,
  "expects_training_arg": false,
  "dtype": "float32",
  "batch_input_shape": null,
  "autocast": false,
  "class_name": "Concatenate",
  "config": {},
  "shared_object_id": 6,
  "build_input_shape": {
    "class_name": "TensorShape",
    "items": [
      null,
      128
    ]
  }
}2
ëroot.layer_with_weights-2"_tf_keras_layer*´{
  "name": "dense",
  "trainable": true,
  "expects_training_arg": false,
  "dtype": "float32",
  "batch_input_shape": null,
  "autocast": false,
  "input_spec": {
    "class_name": "InputSpec",
    "config": {
      "DType": null,
      "Shape": null,
      "Ndim": null,
      "MinNdim": 2,
      "MaxNdim": null,
      "Axes": {
        "-1": 256
      }
    },
    "shared_object_id": 7
  },
  "class_name": "Dense",
  "config": {
    "units": 256,
    "activation": "relu",
    "use_bias": true,
    "kernel_initializer": {
      "class_name": "GlorotUniform",
      "config": {
        "seed": null
      }
    },
    "bias_initializer": {
      "class_name": "Zeros",
      "config": {}
    },
    "kernel_regularizer": null,
    "bias_regularizer": null,
    "kernel_constraint": null,
    "bias_constraint": null,
    "name": null,
    "dtype": "float32",
    "trainable": true
  },
  "shared_object_id": 8,
  "build_input_shape": {
    "class_name": "TensorShape",
    "items": [
      null,
      256
    ]
  }
}2
„	root.layer-8"_tf_keras_layer*Ú{
  "name": "dropout",
  "trainable": true,
  "expects_training_arg": false,
  "dtype": "float32",
  "batch_input_shape": null,
  "autocast": false,
  "class_name": "Dropout",
  "config": {
    "rate": 0.3,
    "noise_shape": null,
    "seed": null,
    "name": null,
    "dtype": "float32",
    "trainable": true
  },
  "shared_object_id": 9,
  "build_input_shape": {
    "class_name": "TensorShape",
    "items": [
      null,
      256
    ]
  }
}2
ï
root.layer_with_weights-3"_tf_keras_layer*¸{
  "name": "dense_1",
  "trainable": true,
  "expects_training_arg": false,
  "dtype": "float32",
  "batch_input_shape": null,
  "autocast": false,
  "input_spec": {
    "class_name": "InputSpec",
    "config": {
      "DType": null,
      "Shape": null,
      "Ndim": null,
      "MinNdim": 2,
      "MaxNdim": null,
      "Axes": {
        "-1": 256
      }
    },
    "shared_object_id": 10
  },
  "class_name": "Dense",
  "config": {
    "units": 128,
    "activation": "relu",
    "use_bias": true,
    "kernel_initializer": {
      "class_name": "GlorotUniform",
      "config": {
        "seed": null
      }
    },
    "bias_initializer": {
      "class_name": "Zeros",
      "config": {}
    },
    "kernel_regularizer": null,
    "bias_regularizer": null,
    "kernel_constraint": null,
    "bias_constraint": null,
    "name": null,
    "dtype": "float32",
    "trainable": true
  },
  "shared_object_id": 11,
  "build_input_shape": {
    "class_name": "TensorShape",
    "items": [
      null,
      256
    ]
  }
}2
ˆroot.layer-10"_tf_keras_layer*Ý{
  "name": "dropout_1",
  "trainable": true,
  "expects_training_arg": false,
  "dtype": "float32",
  "batch_input_shape": null,
  "autocast": false,
  "class_name": "Dropout",
  "config": {
    "rate": 0.3,
    "noise_shape": null,
    "seed": null,
    "name": null,
    "dtype": "float32",
    "trainable": true
  },
  "shared_object_id": 12,
  "build_input_shape": {
    "class_name": "TensorShape",
    "items": [
      null,
      128
    ]
  }
}2
ïroot.layer_with_weights-4"_tf_keras_layer*¸{
  "name": "dense_2",
  "trainable": true,
  "expects_training_arg": false,
  "dtype": "float32",
  "batch_input_shape": null,
  "autocast": false,
  "input_spec": {
    "class_name": "InputSpec",
    "config": {
      "DType": null,
      "Shape": null,
      "Ndim": null,
      "MinNdim": 2,
      "MaxNdim": null,
      "Axes": {
        "-1": 128
      }
    },
    "shared_object_id": 13
  },
  "class_name": "Dense",
  "config": {
    "units": 1,
    "activation": "linear",
    "use_bias": true,
    "kernel_initializer": {
      "class_name": "GlorotUniform",
      "config": {
        "seed": null
      }
    },
    "bias_initializer": {
      "class_name": "Zeros",
      "config": {}
    },
    "kernel_regularizer": null,
    "bias_regularizer": null,
    "kernel_constraint": null,
    "bias_constraint": null,
    "name": null,
    "dtype": "float32",
    "trainable": true
  },
  "shared_object_id": 14,
  "build_input_shape": {
    "class_name": "TensorShape",
    "items": [
      null,
      128
    ]
  }
}2