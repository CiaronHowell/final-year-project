import React from 'react';

export const Context = React.createContext();

export default class ContextProvider extends React.Component {
    state = {running: false}

    render() {
        return (
            <Context.Provider value={
                {
                    state: this.state,
                    setMessage: (value) => this.setState({
                        running: value
                    })
                }
            }>
                {this.props.children}
            </Context.Provider>
        )
    }
}