#ifndef _OFC_VECTOR_HPP_
#define _OFC_VECTOR_HPP_

#include <array>
#include <algorithm>
#include <functional>
#include <cassert>
#include <typeinfo>

#define VECX 0
#define VECY 1
#define VECZ 2
#define VECW 3

namespace OFC
{
    template<size_t dimension> concept c_IsVector2D = (dimension == 2);
    template<size_t dimension> concept c_IsVector3D = (dimension == 3);
    template<size_t dimension> concept c_IsVector4D = (dimension == 4);
    template<size_t dimension> concept c_IsValidVector = (c_IsVector2D<dimension> || c_IsVector3D<dimension> || c_IsVector4D<dimension>);

    /**
     * @brief An N Dimensional, arithmetic-type vector
     * @authors MrGrim, TSB
    **/
    template<class vecType, size_t vecDimension> 
    requires std::is_arithmetic_v<vecType> && c_IsValidVector<vecDimension> && std::is_signed_v<vecType>
    struct Vector
    {
        using vecStorageType = std::array<vecType, vecDimension>;
        vecStorageType components;

        Vector() = default;

        /**
         * @brief Construct an N Dimensional vector from a sequence of arithmetic types
        **/
        template<class... compTypes>
        requires std::is_same_v<std::common_type_t<compTypes...>, vecType> && (vecDimension == sizeof...(compTypes))
        explicit Vector (compTypes... values) 
        {
            components = { values... };
        }

        /**
         * @brief Construct an N dimensional vector from another vector or compatible storage type.
        **/
        explicit Vector(vecStorageType &&values)
        {
            components = std::move(values);
        }

        /**
         * @brief Overloaded subscript operator for vector component get
        **/
        vecType& operator [](const size_t vecComponent)
        {
            return components[vecComponent];
        }

        /**
         * @brief Cast between two vectors, assuming that the vector dimensions are the same.
        **/
        template<template<class, size_t> class castTo, class castType, size_t castDimension>
        requires std::is_arithmetic_v<castType> && (castDimension == vecDimension) && 
        std::is_same_v<castTo<castType, castDimension>, Vector<castType, castDimension>>
        operator castTo<castType, castDimension> () const
        {
            castTo<castType, castDimension> castedData;

            size_t i = 0;
            while(i < vecDimension)
            {
                castedData[i] = static_cast<castType>(components[i]);
                ++i;
            }

            return castedData;
        }

        /**
         * @brief Convert vector to a different size
        **/
        template<size_t dimensionTo>
        requires (dimensionTo != vecDimension)
        Vector<vecType, dimensionTo> To()
        {
            Vector<vecType, dimensionTo> result;
            result.components.fill(0);
            
            size_t i = 0;
            do {
                result[i] = components[i];
                ++i;
            } while((i < dimensionTo && i < vecDimension));

            return result;
        }

        /**
         * @brief Add two vectors together.
        **/
        template<class addType, size_t addDimension, class resultType = std::common_type_t<vecType, addType>>
        //requires (vecDimension == addedDimension)
        Vector<resultType, vecDimension> operator+(const Vector<addType, addDimension> &addedVec)
        {
            Vector<resultType, vecDimension> result;

            size_t i = 0;
            do {
                result[i] = components[i] + addedVec.components[i];
                ++i;
            } while((i < addDimension && i < vecDimension));

            return result;
        }

        /**
         * @brief Subtracts two vectors
        **/
        template<class subType, size_t subDimension, class resultType = std::common_type_t<vecType, subType>>
        //requires (vecDimension == subDimension)
        Vector<resultType, vecDimension> operator-(const Vector<subType, subDimension> &subtractedVec)
        {
            Vector<resultType, vecDimension> result;

            size_t i = 0;
            do {
                result[i] = components[i] - subtractedVec.components[i];
                ++i;
            } while((i < subDimension && i < vecDimension));

            return result;
        }

        
    };
}

#endif