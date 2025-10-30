"use client"
import * as React from "react"
import { Slot } from "@radix-ui/react-slot"
import { Controller, FormProvider, useFormContext, useFormState } from "react-hook-form"
import { cn } from "../../lib/utils"
import { Label } from "./label"

const Form = FormProvider

/**
 * @template {import("react-hook-form").FieldValues} TFieldValues
 * @template {import("react-hook-form").FieldPath<TFieldValues>} TName
 * @typedef {Object} FormFieldContextValue
 * @property {TName} name
 */

/** @type {React.Context<FormFieldContextValue>} */
const FormFieldContext = React.createContext({})

/**
 * @template {import("react-hook-form").FieldValues} TFieldValues
 * @template {import("react-hook-form").FieldPath<TFieldValues>} TName
 * @param {import("react-hook-form").ControllerProps<TFieldValues, TName>} props
 */
const FormField = ({ ...props }) => {
    return (
        <FormFieldContext.Provider value={ { name: props.name } }>
            <Controller { ...props } />
        </FormFieldContext.Provider>
    )
}

/**
 * @returns {{
 *   id: string,
 *   name: string,
 *   formItemId: string,
 *   formDescriptionId: string,
 *   formMessageId: string,
 *   invalid: boolean,
 *   isDirty: boolean,
 *   isTouched: boolean,
 *   error?: import("react-hook-form").FieldError
 * }}
 */
const useFormField = () => {
    const fieldContext = React.useContext(FormFieldContext)
    const itemContext = React.useContext(FormItemContext)
    const { getFieldState } = useFormContext()
    const formState = useFormState({ name: fieldContext.name })
    const fieldState = getFieldState(fieldContext.name, formState)

    if (!fieldContext) {
        throw new Error("useFormField should be used within <FormField>")
    }

    const { id } = itemContext

    return {
        id,
        name: fieldContext.name,
        formItemId: `${ id }-form-item`,
        formDescriptionId: `${ id }-form-item-description`,
        formMessageId: `${ id }-form-item-message`,
        ...fieldState
    }
}

/**
 * @typedef {Object} FormItemContextValue
 * @property {string} id
 */

/** @type {React.Context<FormItemContextValue>} */
const FormItemContext = React.createContext({})

/**
 * @param {React.ComponentProps<"div">} props
 */
function FormItem({ className, ...props }) {
    const id = React.useId()

    return (
        <FormItemContext.Provider value={ { id } }>
            <div
                data-slot='form-item'
                className={ cn("grid gap-2", className) }
                { ...props }
            />
        </FormItemContext.Provider>
    )
}

/**
 * @param {React.ComponentProps<typeof LabelPrimitive.Root>} props
 */
function FormLabel({ className, ...props }) {
    const { error, formItemId } = useFormField()

    return (
        <Label
            data-slot='form-label'
            data-error={ !!error }
            className={ cn("data-[error=true]:text-destructive", className) }
            htmlFor={ formItemId }
            { ...props }
        />
    )
}

/**
 * @param {React.ComponentProps<typeof Slot>} props
 */
function FormControl({ ...props }) {
    const { error, formItemId, formDescriptionId, formMessageId } = useFormField()

    return (
        <Slot
            data-slot='form-control'
            id={ formItemId }
            aria-describedby={
                !error
                    ? `${ formDescriptionId }`
                    : `${ formDescriptionId } ${ formMessageId }`
            }
            aria-invalid={ !!error }
            { ...props }
        />
    )
}

/**
 * @param {React.ComponentProps<"p">} props
 */
function FormDescription({ className, ...props }) {
    const { formDescriptionId } = useFormField()

    return (
        <p
            data-slot='form-description'
            id={ formDescriptionId }
            className={ cn("text-muted-foreground text-sm", className) }
            { ...props }
        />
    )
}

/**
 * @param {React.ComponentProps<"p">} props
 */
function FormMessage({ className, ...props }) {
    const { error, formMessageId } = useFormField()
    const body = error ? String(error?.message ?? "") : props.children

    if (!body) {
        return null
    }

    return (
        <p
            data-slot='form-message'
            id={ formMessageId }
            className={ cn("text-destructive text-sm", className) }
            { ...props }
        >
            { body }
        </p>
    )
}

export {
    // eslint-disable-next-line react-refresh/only-export-components
    useFormField,
    Form,
    FormItem,
    FormLabel,
    FormControl,
    FormDescription,
    FormMessage,
    FormField
}