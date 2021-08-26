LOCAL_PATH:= $(call my-dir)
include $(CLEAR_VARS)

LOCAL_C_INCLUDES := ../gamelink-cpp/c_library ../gamelink-cpp/include ../gamelink-cpp/third_party  ../gamelink-cpp
LOCAL_SRC_FILES := $(wildcard ../gamelink-cpp/schema/*.cpp) $(wildcard ../gamelink-cpp/src/*.cpp) $(wildcard ../gamelink-cpp/c_library/*.cpp)
LOCAL_MODULE := cgamelink
include $(BUILD_SHARED_LIBRARY)